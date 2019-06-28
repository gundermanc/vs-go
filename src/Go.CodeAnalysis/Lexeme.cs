namespace Go.CodeAnalysis
{
    using Microsoft.VisualStudio.Text;

    public struct Lexeme
    {
        public Lexeme(SnapshotSpan span)
        {
            this.Span = span;
        }

        public SnapshotSpan Span { get;  }
    }
}
