namespace Go.CodeAnalysis.Text
{
    /// <summary>
    /// A text snapshot based on a concrete string.
    /// </summary>
    public sealed class StringSnapshot : SnapshotBase
    {
        /// <summary>
        /// String content of this text snapshot.
        /// </summary>
        private readonly string text;

        public StringSnapshot(string text) : base(text.Length)
        {
            this.text = text;
        }

        public override char this[int offset] => this.text[offset];
    }
}
