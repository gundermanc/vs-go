namespace Go.CodeAnalysis.Semantic
{
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Text;

    public sealed class ImportDeclarationSemanticNode : ImportDeclarationNode
    {
        public ImportDeclarationSemanticNode(
            SnapshotSegment code,
            SnapshotSegment packageName) : base(code, packageName)
        {
        }
    }
}
