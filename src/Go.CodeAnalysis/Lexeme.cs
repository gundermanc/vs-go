namespace Go.CodeAnalysis
{
    public struct Lexeme
    {
        public Lexeme(SnapshotSegment segment, LexemeType type)
        {
            this.Segment = segment;
            this.Type = type;
        }

        public SnapshotSegment Segment { get;  }

        public LexemeType Type { get; }
    }
}
