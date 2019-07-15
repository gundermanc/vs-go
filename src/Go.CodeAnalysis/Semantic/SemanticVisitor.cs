namespace Go.CodeAnalysis.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Parser;

    internal sealed class SemanticVisitor : DepthFirstTraversalVisitorBase
    {
        private readonly ParseSnapshot parseSnapshot;
        private readonly Stack<ParseNodeBase> stack = new Stack<ParseNodeBase>();
        private readonly ImmutableArray<Error>.Builder errors = ImmutableArray.CreateBuilder<Error>();

        public SemanticVisitor(ParseSnapshot parseSnapshot)
        {
            this.parseSnapshot = parseSnapshot
                ?? throw new ArgumentNullException(nameof(parseSnapshot));
        }

        public (DocumentSemanticNode, ImmutableArray<Error>) CreateSemanticTree()
        {
            this.Visit(this.parseSnapshot.RootNode);

            if (this.stack.Count != 1)
            {
                throw new InvalidOperationException("Internal error in semantic analysis.");
            }

            return ((DocumentSemanticNode)this.stack.Pop(), this.errors.ToImmutable());
        }

        public override void Visit(DocumentNode parseNode)
        {
            base.Visit(parseNode);

            var documentBodyNode = (DocumentBodySemanticNode)this.stack.Pop();
            var importsNode = (ImportsSemanticNode)this.stack.Pop();
            var packageDeclarationNode = (PackageDeclarationSemanticNode)this.stack.Pop();

            this.stack.Push(new DocumentSemanticNode(parseNode.Extent, packageDeclarationNode, importsNode, documentBodyNode));
        }

        public override void Visit(PackageDeclarationNode parseNode)
        {
            base.Visit(parseNode);
            this.stack.Push(new PackageDeclarationSemanticNode(parseNode.Extent, parseNode.PackageNameExtent));
        }

        public override void Visit(ImportsNode parseNode)
        {
            base.Visit(parseNode);

            this.stack.Push(
                new ImportsSemanticNode(
                    parseNode.Extent,
                    this.PopAll<ImportNode>()));
        }

        public override void Visit(ImportNode parseNode)
        {
            base.Visit(parseNode);

            // TODO: do this right for multi-line imports.
            this.stack.Push(
                new ImportSemanticNode(
                    parseNode.Extent,
                    this.PopAll<ImportDeclarationNode>()));
        }

        public override void Visit(ImportDeclarationNode parseNode)
        {
            base.Visit(parseNode);

            this.stack.Push(new ImportDeclarationSemanticNode(parseNode.Extent, parseNode.PackageName));
        }

        public override void Visit(DocumentBodyNode parseNode)
        {
            base.Visit(parseNode);

            this.stack.Push(
                new DocumentBodySemanticNode(
                    parseNode.Extent,
                    this.PopAll<FunctionDeclarationNode>().CastArray<ParseNodeBase>()));
        }

        public override void Visit(FunctionDeclarationNode parseNode)
        {
            base.Visit(parseNode);
            this.stack.Push(
                new FunctionDeclarationSemanticNode(
                    parseNode.Extent,
                    parseNode.FunctionNameExtent,
                    (BlockSemanticNode)this.stack.Pop()));
        }

        public override void Visit(BlockNode parseNode)
        {
            base.Visit(parseNode);
            this.stack.Push(new BlockSemanticNode(parseNode.Extent));
        }

        private ImmutableArray<TNode> PopAll<TNode>() where TNode : ParseNodeBase
        {
            var builder = ImmutableArray.CreateBuilder<TNode>();
            while (this.stack.Count > 0 && this.stack.Peek() is TNode)
            {
                var node = (TNode)this.stack.Pop();

                builder.Add(node);
            }

            // Stack reverses the order. Reverse to get them back in order.
            builder.Reverse();

            return builder.ToImmutable();
        }
    }
}
