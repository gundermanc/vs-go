namespace Go.CodeAnalysis.Common
{
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Represents an error in the selected text.
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
