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
                        if (this.TryConsumeSpan(this.ConsumeEndLine, LexemeType.LineComment, out lexeme)) return true;
                        break;
                    case '*':
                        if (this.TryConsumeSpan(this.ConsumeEndComment, LexemeType.GeneralComment, out lexeme)) return true;
                        break;
                }
            }

            lexeme = default;
            return false;
        }

        private (int take, int skip) ConsumeEndComment(int offset)
        {
            if (this.currentSnapshot[offset] == '*' &&
                this.TryPeekNext(out var next) &&
                next == '/')
            {
                return (2, 0);
            }

            return (0, 0);
        }

        // Length adjustment is a hack for now to make dealing with multi-char newlines easier.
        private bool TryConsumeSpan(
            Func<int, (int take, int skip)> consumeFunc,
            LexemeType type,
            out Lexeme lexeme)
        {
            int start = this.currentOffset;
            int take = 0;
            int skip = 0;

            while (this.currentOffset < this.currentSnapshot.Length)
            {
                (take, skip) = consumeFunc(this.currentOffset);
                if (take > 0 || skip > 0)
                {
                    break;
                }

                this.currentOffset++;
            }

            this.currentOffset += take;

            var segmentLength = this.currentOffset - start;
            if (segmentLength > 0)
            {
                var segment = new SnapshotSegment(this.currentSnapshot, start, segmentLength);
                lexeme = new Lexeme(segment, type);
                this.currentOffset += skip;
                return true;
            }
            else
            {
                lexeme = default;
                return false;
            }
        }

        private (int take, int skip) ConsumeEndLine(int offset)
        {
            if (this.currentSnapshot[offset] == '\r')
            {
                if (this.TryPeekNext(out var next) && next == '\n')
                {
                    return (0, 2);
                }
                else
                {
                    return (0, 1);
                }
            }
            else if (this.currentSnapshot[offset] == '\n')
            {
                return (0, 1);
            }
            else
            {
                return (0, 0);
            }
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
