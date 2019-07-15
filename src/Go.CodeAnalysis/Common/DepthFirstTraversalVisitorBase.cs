namespace Go.CodeAnalysis.Common
{
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Parser;

    internal abstract class DepthFirstTraversalVisitorBase : IVisitor
    {
        public virtual void Visit(DocumentNode parseNode)
        {
            parseNode.PackageDeclaration.Accept(this);
            parseNode.ImportsNode.Accept(this);
            parseNode.DocumentBody.Accept(this);
        }

        public virtual void Visit(PackageDeclarationNode parseNode) { }

        public virtual void Visit(ImportsNode parseNode) => this.VisitNodes(parseNode.Imports);

        public virtual void Visit(ImportNode parseNode) { }

        public virtual void Visit(DocumentBodyNode parseNode) => this.VisitNodes(parseNode.Declarations);

        public virtual void Visit(FunctionDeclarationNode parseNode) => parseNode.BlockNode.Accept(this);

        public virtual void Visit(BlockNode parseNode) { }

        public void Visit(ImportDeclarationNode parseNode) { }

        private void VisitNodes<TNode>(ImmutableArray<TNode> nodes) where TNode : ParseNodeBase
        {
            foreach (var node in nodes)
            {
                node.Accept(this);
            }
        }
    }
}
