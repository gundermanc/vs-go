namespace Go.CodeAnalysis.Text
{
    /// <summary>
    /// Abstract representation of a text segment.
    /// </summary>
    public abstract class SnapshotBase
    {
        protected SnapshotBase(int length)
        {
            this.Length = length;
        }

        /// <summary>
        /// Gets the character located at a specific index in the text segment.
        /// </summary>
        /// <param name="offset"> Index of character in the text segment. </param>
        /// <returns></returns>
        public abstract char this[int offset] { get; }

        /// <summary>
        /// Create an instance of concrete subclass. 
        /// </summary>
        public SnapshotSegment Extent => new SnapshotSegment(this, 0, this.Length);

        /// <summary>
        /// Length of the text segment.
        /// </summary>
        public int Length { get; }
    }
}
