using System;

namespace Go.CodeAnalysis
{
    public sealed class Lexer
    {
        private readonly SnapshotBase currentSnapshot;
        private int currentOffset;

        public static Lexer Create(SnapshotBase currentSnapshot)
        {
            return new Lexer(currentSnapshot, 0);
        }

        private Lexer(SnapshotBase currentSnapshot, int currentOffset)
        {
            this.currentSnapshot = currentSnapshot;
            this.currentOffset = currentOffset;
        }

        public bool TryGetNextLexeme(out Lexeme lexeme)
        {
            while (this.currentOffset < this.currentSnapshot.Length)
            {
                if (this.TryConsumeLexeme(out lexeme))
                {
                    return true;
                }
            }

            lexeme = default;
            return false;
        }

        private bool TryConsumeLexeme(out Lexeme lexeme)
        {
            bool consumedSomething = false;

            switch (this.currentSnapshot[this.currentOffset])
            {
                // Looks like the start of a comment.
                case '/':
                    if (this.TryConsumeLeadingSlash(out lexeme)) return true;
                    break;
            }

            // Keep us moving if we didn't find anything.
            if (!consumedSomething)
            {
                this.currentOffset++;
            }

            lexeme = default;
            return false;
        }

        private bool TryConsumeLeadingSlash(out Lexeme lexeme)
        {
            // Two slashes.. '//' means the start of a line comment.
            if (this.TryPeekNext(out var next))
            {
                switch (next)
                {
                    case '/':
                        if (this.TryConsumeUntil(this.IsEndLine, LexemeType.LineComment, out lexeme)) return true;
                        return true;
                }
            }

            lexeme = default;
            return false;
        }

        private bool TryConsumeUntil(Func<char, bool> shouldStop, LexemeType type, out Lexeme lexeme)
        {
            int start = this.currentOffset;

            while (this.currentOffset < this.currentSnapshot.Length &&
                !shouldStop(this.currentSnapshot[this.currentOffset]))
            {
                this.currentOffset++;
            }

            var segmentLength = this.currentOffset - start;
            if (segmentLength > 0)
            {
                var segment = new SnapshotSegment(this.currentSnapshot, start, segmentLength);
                lexeme = new Lexeme(segment, type);
                return true;
            }
            else
            {
                lexeme = default;
                return false;
            }
        }

        private bool IsEndLine(char candidate)
        {
            // TODO: for mac, we need to consume \r.
            return /*candidate == '\r' ||*/ candidate == '\n';
        }

        private bool TryPeekNext(out char next)
        {
            if (this.currentOffset + 1 >= this.currentSnapshot.Length)
            {
                next = default;
                return false;
            }

            next = this.currentSnapshot[currentOffset + 1];
            return true;
        }
    }
}
