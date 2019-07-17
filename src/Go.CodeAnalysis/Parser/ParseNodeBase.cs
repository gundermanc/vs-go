namespace Go.CodeAnalysis.Parser
{
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Represents a node in abstract syntax tree.
    /// </summary>
    public abstract class ParseNodeBase
    {
        public ParseNodeBase(SnapshotSegment extent)
        {
            this.Extent = extent;
        }

        /// <summary>
        /// Code segment in this node.
        /// </summary>
        public SnapshotSegment Extent { get; }

        public abstract void Accept(IVisitor visitor);
    }
}
