namespace Go.CodeAnalysis.Semantic
{
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Text;

    public class BlockSemanticNode : BlockNode
    {
        public BlockSemanticNode(SnapshotSegment extent) : base(extent)
        {
        }
    }
}
