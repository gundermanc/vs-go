namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Represents a node in abstract syntax tree.
    /// </summary>
    public class ParseNode
    {
        public ParseNode(SnapshotSegment extent, ImmutableArray<ParseNode> children)
        {
            this.Extent = extent;
            this.Children = children;
        }

        /// <summary>
        /// Code segment in this node.
        /// </summary>
        public SnapshotSegment Extent { get; }

        // TODO: move to a sub class?
        public ImmutableArray<ParseNode> Children { get; }
    }
}
