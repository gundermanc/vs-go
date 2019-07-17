namespace Go.CodeAnalysis.Semantic
{
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Text;

    public sealed class DocumentBodySemanticNode : DocumentBodyNode
    {
        public DocumentBodySemanticNode(
            SnapshotSegment extent,
            ImmutableArray<ParseNodeBase> declarations)
            : base(extent, declarations)
        {
        }
    }
}
