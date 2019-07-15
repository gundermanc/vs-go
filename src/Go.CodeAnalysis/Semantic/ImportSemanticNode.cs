namespace Go.CodeAnalysis.Semantic
{
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Text;

    public sealed class ImportSemanticNode : ImportNode
    {
        public ImportSemanticNode(
            SnapshotSegment extent,
            ImmutableArray<ImportDeclarationNode> importDeclarations)
            : base(extent, importDeclarations)
        {
        }
    }
}
