namespace Go.CodeAnalysis
{
    using System;

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
                else
                {
                    // Keep us moving if we didn't find anything.
                    this.currentOffset++;
                }
            }

            lexeme = default;
            return false;
        }

        private bool TryConsumeLexeme(out Lexeme lexeme)
        {
            switch (this.currentSnapshot[this.currentOffset])
            {
                case '+':
                    if (this.TryConsumeMultiCharacterSymbol('+', '=', out lexeme)) return true;
                    goto Symbol;
                case '-':
                    if (this.TryConsumeMultiCharacterSymbol('-', '=', out lexeme)) return true;
                    goto Symbol;
                case '*':
                    if (this.TryConsumeMultiCharacterSymbol('*', '=', out lexeme)) return true;
                    goto Symbol;
                case '/':
                    if (this.TryConsumeMultiCharacterSymbol('/', '=', out lexeme)) return true;
                    if (this.TryConsumeLeadingSlash(out lexeme)) return true;
                    goto Symbol;
                case '%':
                    if (this.TryConsumeMultiCharacterSymbol('%', '=', out lexeme)) return true;
                    goto Symbol;
                case '&':
                    if (this.TryConsumeMultiCharacterSymbol('&', '^', out lexeme)) return true;
                    if (this.TryConsumeMultiCharacterSymbol('&', '=', out lexeme)) return true;
                    goto Symbol;
                case '|':
                    if (this.TryConsumeMultiCharacterSymbol('|', '=', out lexeme)) return true;
                    goto Symbol;
                case '^':
                    if (this.TryConsumeMultiCharacterSymbol('^', '=', out lexeme)) return true;
                    goto Symbol;
                case '<':
                    if (this.TryConsumeMultiCharacterSymbol('<', '<', out lexeme)) return true;
                    goto Symbol;
                case '>':
                    if (this.TryConsumeMultiCharacterSymbol('>', '>', out lexeme)) return true;
                    goto Symbol;
                case '=':
                case '!':
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                case ',':
                case ';':
                case '.':
                case ':':
                Symbol:
                    lexeme = new Lexeme(new SnapshotSegment(this.currentSnapshot, this.currentOffset++, 1), LexemeType.Operator);
                    return true;
                case '"':
                    if (this.TryConsumeSpan(this.ConsumeStringLiteral, LexemeType.String, out lexeme)) return true;
                    break;
                case ' ':
                case '\t':
                case '\v':
                case '\r':
                case '\n':
                    break;
                default:
                    if (this.TryConsumeKeywordLiteralOrIdentifier(out lexeme)) return true;
                    break;
            }

            lexeme = default;
            return false;
        }

        private bool TryConsumeKeywordLiteralOrIdentifier(out Lexeme lexeme)
        {
            if (char.IsDigit(this.currentSnapshot[this.currentOffset]))
            {
                return this.TryConsumeInteger(out lexeme);
            }
            else
            {
                return this.TryConsumeKeywordOrIdentifier(out lexeme);
            }
        }

        private bool TryConsumeInteger(out Lexeme lexeme)
        {
            int start = this.currentOffset;
            int i;
            for (i = this.currentOffset; i < this.currentSnapshot.Length && char.IsDigit(this.currentSnapshot[i]); i++);

            var length = i - start;
            if (length > 0)
            {
                this.currentOffset += length;
                lexeme = new Lexeme(new SnapshotSegment(this.currentSnapshot, start, length), LexemeType.Integer);
                return true;
            }

            lexeme = default;
            return false;
        }

        private bool TryConsumeKeywordOrIdentifier(out Lexeme lexeme)
        {
            if (this.TryConsumeSpan(this.ConsumeKeywordOrIdentifer, LexemeType.Identifier, out lexeme))
            {
                lexeme = this.IdentifierToKeyword(lexeme);
                return true;
            }

            lexeme = default;
            return false;
        }

        private Lexeme IdentifierToKeyword(Lexeme lexeme)
        {
            Lexeme LexemeForKeyword(string keyword)
            {
                if (this.SegmentEquals(lexeme.Segment, keyword))
                {
                    return new Lexeme(lexeme.Segment, LexemeType.Keyword);
                }

                return lexeme;
            }

            switch (lexeme.Segment[0])
            {
                case 'b':
                    return LexemeForKeyword("break");
                case 'c':
                    if (this.SegmentEquals(lexeme.Segment, "case") ||
                        this.SegmentEquals(lexeme.Segment, "chan") ||
                        this.SegmentEquals(lexeme.Segment, "const") ||
                        this.SegmentEquals(lexeme.Segment, "continue"))
                    {
                        return new Lexeme(lexeme.Segment, LexemeType.Keyword);
                    }
                    break;
                case 'd':
                    if (this.SegmentEquals(lexeme.Segment, "default") ||
                        this.SegmentEquals(lexeme.Segment, "defer"))
                    {
                        return new Lexeme(lexeme.Segment, LexemeType.Keyword);
                    }
                    break;
                case 'e':
                    return LexemeForKeyword("else");
                case 'f':
                    if (this.SegmentEquals(lexeme.Segment, "fallthrough") ||
                        this.SegmentEquals(lexeme.Segment, "for") ||
                        this.SegmentEquals(lexeme.Segment, "func"))
                    {
                        return new Lexeme(lexeme.Segment, LexemeType.Keyword);
                    }
                    break;
                case 'g':
                    if (this.SegmentEquals(lexeme.Segment, "go") ||
                        this.SegmentEquals(lexeme.Segment, "goto"))
                    {
                        return new Lexeme(lexeme.Segment, LexemeType.Keyword);
                    }
                    break;
                case 'i':
                    if (this.SegmentEquals(lexeme.Segment, "if") ||
                        this.SegmentEquals(lexeme.Segment, "import") ||
                        this.SegmentEquals(lexeme.Segment, "interface"))
                    {
                        return new Lexeme(lexeme.Segment, LexemeType.Keyword);
                    }
                    break;
                case 'm':
                    return LexemeForKeyword("map");
                case 'p':
                    return LexemeForKeyword("package");
                case 'r':
                    if (this.SegmentEquals(lexeme.Segment, "range") ||
                        this.SegmentEquals(lexeme.Segment, "return"))
                    {
                        return new Lexeme(lexeme.Segment, LexemeType.Keyword);
                    }
                    break;
                case 's':
                    if (this.SegmentEquals(lexeme.Segment, "select") ||
                        this.SegmentEquals(lexeme.Segment, "struct") ||
                        this.SegmentEquals(lexeme.Segment, "switch"))
                    {
                        return new Lexeme(lexeme.Segment, LexemeType.Keyword);
                    }
                    break;
                case 't':
                    return LexemeForKeyword("type");
                case 'v':
                    return LexemeForKeyword("var");
            }

            return lexeme;
        }

        private bool SegmentEquals(SnapshotSegment segment, string text)
        {
            if (segment.Length != text.Length)
            {
                return false;
            }

            for (int i = 0; i < segment.Length; i++)
            {
                if (segment[i] != text[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryConsumeMultiCharacterSymbol(char first, char second, out Lexeme lexeme)
        {
            if (this.currentSnapshot[this.currentOffset] == first &&
                this.TryPeekNext(out var next) &&
                next == second)
            {
                lexeme = new Lexeme(new SnapshotSegment(this.currentSnapshot, this.currentOffset, 2), LexemeType.Operator);
                this.currentOffset += 2;
                return true;
            }
            else
            {
                lexeme = default;
                return false;
            }
        }

        private (int take, int skip) ConsumeKeywordOrIdentifer(int offset)
        {
            if (!char.IsLetter(this.currentSnapshot[offset]))
            {
                return (0, 0);
            }

            int i = offset + 1;
            while (i < this.currentSnapshot.Length && char.IsLetterOrDigit(this.currentSnapshot[i]))
            {
                i++;
            }

            return (i - offset, 0);
        }

        private (int take, int skip) ConsumeStringLiteral(int offset)
        {
            if (this.currentSnapshot[offset] == '"')
            {
                return (1, 0);
            }
            else
            {
                return (0, 0);
            }
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

            currentOffset++;

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

            next = this.currentSnapshot[this.currentOffset + 1];
            return true;
        }
    }
}
