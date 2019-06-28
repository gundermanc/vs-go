namespace Go.CodeAnalysis
{
    using Microsoft.VisualStudio.Text;

    public sealed class Lexer
    {
        private readonly ITextSnapshot currentSnapshot;
        private SnapshotPoint currentOffset;

        public static Lexer Create(ITextSnapshot currentSnapshot)
        {
            return new Lexer(currentSnapshot, new SnapshotPoint(currentSnapshot, 0));
        }

        private Lexer(ITextSnapshot currentSnapshot, SnapshotPoint currentOffset)
        {
            this.currentSnapshot = currentSnapshot;
            this.currentOffset = currentOffset;
        }

        public bool TryGetNextLexeme(out Lexeme lexeme)
        {
            if (this.currentOffset.Position >= this.currentSnapshot.Length)
            {
                lexeme = default;
                return false;
            }

            // For now, just do some silly span highlighting for the purposes of lighting
            // up some of the editor features.
            if (this.currentSnapshot.Length < 5)
            {
                lexeme = default;
                return false;
            }

            lexeme = new Lexeme(new SnapshotSpan(this.currentSnapshot, 0, 4));
            this.currentOffset = new SnapshotPoint(this.currentSnapshot, this.currentSnapshot.Length);
            return true;
        }
    }
}
