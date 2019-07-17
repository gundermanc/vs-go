namespace Go.CodeAnalysis.Semantic
{
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Text;

    public sealed class PackageDeclarationSemanticNode : PackageDeclarationNode
    {
        public PackageDeclarationSemanticNode(
            SnapshotSegment extent,
            SnapshotSegment packageNameExtent)
            : base(extent, packageNameExtent)
        {
        }
    }
}
