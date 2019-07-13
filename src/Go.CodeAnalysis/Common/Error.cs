namespace Go.CodeAnalysis.Common
{
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Items in Error List.
    /// </summary>
    public struct Error
    {
        public Error(SnapshotSegment extent, string message)
        {
            this.Extent = extent;
            this.Message = message;
        }

        /// <summary>
        /// Code segment that contains error.
        /// </summary>
        public SnapshotSegment Extent { get; }

        /// <summary>
        /// Error description.
        /// </summary>
        public string Message { get; }
    }
}
