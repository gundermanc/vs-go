namespace Go.CodeAnalysis.Semantic
{
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Text;

    public sealed class DocumentSemanticNode : DocumentNode
    {
        public DocumentSemanticNode(
            SnapshotSegment extent,
            PackageDeclarationNode packageDeclaration,
            ImportsNode importsNode,
            DocumentBodyNode documentBodyNode)
            : base(extent, packageDeclaration, importsNode, documentBodyNode)
        {
        }
    }
}
