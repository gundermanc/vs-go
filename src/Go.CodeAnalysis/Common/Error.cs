namespace Go.CodeAnalysis.Common
{
    using Go.CodeAnalysis.Text;

    public struct Error
    {
        public Error(SnapshotSegment extent, string message)
        {
            this.Extent = extent;
            this.Message = message;
        }

        public SnapshotSegment Extent { get; }

        public string Message { get; }
    }
}
