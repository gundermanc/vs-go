namespace Go.CodeAnalysis.Lex
{
    using Go.CodeAnalysis.Text;

    public struct Lexeme
    {
        public Lexeme(SnapshotSegment segment, LexemeType type)
        {
            this.Extent = segment;
            this.Type = type;
        }

        public SnapshotSegment Extent { get;  }

        public LexemeType Type { get; }
    }
}
