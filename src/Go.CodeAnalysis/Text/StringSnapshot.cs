namespace Go.CodeAnalysis.Text
{
    public sealed class StringSnapshot : SnapshotBase
    {
        private readonly string text;

        public StringSnapshot(string text) : base(text.Length)
        {
            this.text = text;
        }

        public override char this[int offset] => this.text[offset];
    }
}
