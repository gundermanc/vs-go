namespace Go.CodeAnalysis.Lex
{
    using System;
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// A language token.
    /// </summary>
    public struct Lexeme
    {
        public Lexeme(SnapshotSegment segment, LexemeType type)
        {
            this.Extent = segment;
            this.Type = type;
        }

        /// <summary>
        /// Text span of the token.
        /// </summary>
        public SnapshotSegment Extent { get;  }

        /// <summary>
        /// Type of the token.
        /// </summary>
        public LexemeType Type { get; }
    }
}
