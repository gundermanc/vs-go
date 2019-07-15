namespace Go.CodeAnalysis.Common
{
    using System;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Parser;

    internal sealed class PointSearchVisitor : IVisitor
    {
        private readonly ParseSnapshot parseSnapshot;
        private readonly int position;
        private readonly Func<ParseNodeBase, bool> predicate;

        private ParseNodeBase discoveredNode;

        public PointSearchVisitor(ParseSnapshot parseSnapshot, int position, Func<ParseNodeBase, bool> predicate)
        {
            this.parseSnapshot = parseSnapshot
                ?? throw new ArgumentNullException(nameof(parseSnapshot));

            if (position < 0 || position > this.parseSnapshot.RootNode.Extent.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }
            this.position = position;

            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        public ParseNodeBase FindNode()
        {
            this.Visit(this.parseSnapshot.RootNode);

            return this.discoveredNode;
        }

        public void Visit(DocumentNode parseNode)
        {
            if (!parseNode.Extent.ContainsPosition(this.position))
            {
                return;
            }

            if (parseNode?.PackageDeclaration?.Extent.ContainsPosition(this.position) ?? false)
            {
                this.Visit(parseNode.PackageDeclaration);
            }
            else if (parseNode?.ImportsNode?.Extent.ContainsPosition(this.position) ?? false)
            {
                this.Visit(parseNode.ImportsNode);
            }
            else if (parseNode?.DocumentBody?.Extent.ContainsPosition(this.position) ?? false)
            {
                this.Visit(parseNode.DocumentBody);
            }

            if (this.discoveredNode == null)
            {
                this.SetIfApplicable(parseNode);
            }
        }

        public void Visit(PackageDeclarationNode parseNode) => this.SetIfApplicable(parseNode);

        public void Visit(ImportsNode parseNode)
        {
            if (!parseNode.Extent.ContainsPosition(this.position))
            {
                return;
            }

            if (TryFindNode(parseNode.Imports, this.position, out var childNode))
            {
                this.Visit(childNode);
            }
            else
            {
                this.SetIfApplicable(parseNode);
            }
        }

        public void Visit(ImportDeclarationNode parseNode) => this.SetIfApplicable(parseNode);

        public void Visit(ImportNode parseNode) => this.SetIfApplicable(parseNode);

        public void Visit(DocumentBodyNode parseNode)
        {
            if (!parseNode.Extent.ContainsPosition(this.position))
            {
                return;
            }

            if (TryFindNode(parseNode.Declarations, this.position, out var childNode))
            {
                if (childNode is FunctionDeclarationNode functionNode)
                {
                    this.Visit(functionNode);
                }
                else
                {
                    throw new NotImplementedException("Unimplemented case");
                }
            }
            else
            {
                this.SetIfApplicable(parseNode);
            }
        }

        public void Visit(FunctionDeclarationNode parseNode)
        {
            if (!parseNode.Extent.ContainsPosition(this.position))
            {
                return;
            }

            if (parseNode.BlockNode.Extent.ContainsPosition(this.position))
            {
                this.Visit(parseNode.BlockNode);
            }
            else
            {
                this.SetIfApplicable(parseNode);
            }
        }

        public void Visit(BlockNode parseNode) => this.SetIfApplicable(parseNode);

        private void SetIfApplicable(ParseNodeBase parseNode)
        {
            if (this.discoveredNode != null)
            {
                throw new InvalidOperationException($"{nameof(this.discoveredNode)} has already been set.");
            }

            if (parseNode.Extent.ContainsPosition(this.position) &&
                this.predicate(parseNode))
            {
                this.discoveredNode = parseNode;
            }
        }

        // https://en.wikipedia.org/wiki/Binary_search_algorithm
        private static bool TryFindNode<TNode>(
            ImmutableArray<TNode> nodes,
            int position,
            out TNode node) where TNode : ParseNodeBase
        {
            int l = 0;
            int r = nodes.Length - 1;
            while (l <= r)
            {
                int m = (int)Math.Floor((decimal)(l + r) / 2);
                var mExtent = nodes[m].Extent;
                if (!mExtent.ContainsPosition(position))
                {
                    if (mExtent.Start < position)
                    {
                        l = m + 1;
                    }
                    else if (mExtent.End > position)
                    {
                        r = m - 1;
                    }
                }
                else
                {
                    node = nodes[m];
                    return true;
                }
            }

            node = null;
            return false;
        }
    }
}
