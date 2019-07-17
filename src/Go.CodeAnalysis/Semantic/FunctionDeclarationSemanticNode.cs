namespace Go.CodeAnalysis.Semantic
{
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Text;

    public class FunctionDeclarationSemanticNode : FunctionDeclarationNode
    {
        public FunctionDeclarationSemanticNode(
            SnapshotSegment extent,
            SnapshotSegment functionNameExtent,
            BlockNode blockNode)
            : base(extent, functionNameExtent, blockNode)
        {
        }
    }
}
