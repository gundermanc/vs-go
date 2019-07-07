namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Text;

    public class ParseNode
    {
        public ParseNode(SnapshotSegment extent, ImmutableArray<ParseNode> children)
        {
            this.Extent = extent;
            this.Children = children;
        }

        public SnapshotSegment Extent { get; }

        // TODO: move to a sub class?
        public ImmutableArray<ParseNode> Children { get; }
    }
}
