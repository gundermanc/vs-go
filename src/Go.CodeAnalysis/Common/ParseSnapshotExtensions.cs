namespace Go.CodeAnalysis.Common
{
    using Go.CodeAnalysis.Parser;

    public static class ParseSnapshotExtensions
    {
        // TODO: make this a binary search of the extent spans of the AST node children.
        public static bool TryGetDeepestNode<TNode>(this ParseSnapshot parseSnapshot, int position, out TNode foundNode) where TNode : ParseNode
        {
            if (parseSnapshot.RootNode == null)
            {
                foundNode = null;
                return false;
            }

            return parseSnapshot.RootNode.TryGetDeepestNode(position, out foundNode);
        }

        private static bool TryGetDeepestNode<TNode>(this ParseNode parseNode, int position, out TNode foundNode) where TNode : ParseNode
        {
            if (parseNode == null)
            {
                foundNode = null;
                return false;
            }

            if (parseNode.Children.Length > 0)
            {
                foreach (var child in parseNode.Children)
                {
                    if (child.TryGetDeepestNode<TNode>(position, out foundNode))
                    {
                        return true;
                    }
                }
            }

            if (parseNode is TNode concreteNode &&
                concreteNode.Extent.ContainsPosition(position))
            {
                foundNode = concreteNode;
                return true;
            }

            foundNode = null;
            return false;
        }
    }
}
