namespace Go.CodeAnalysis.Semantic
{
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Text;

    public sealed class ImportsSemanticNode : ImportsNode
    {
        public ImportsSemanticNode(
            SnapshotSegment extent,
            ImmutableArray<ImportNode> imports) : base(extent, imports)
        {
        }
    }
}
