namespace Go.CodeAnalysis.Lex
{
    using System;
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Tokenizes a code snapshot.
    /// </summary>
    public sealed class Lexer
    {
        private int currentOffset;

        public static Lexer Create(SnapshotBase currentSnapshot)
        {
            return new Lexer(currentSnapshot, 0);
        }

        private Lexer(SnapshotBase currentSnapshot, int currentOffset)
        {
            this.Snapshot = currentSnapshot;
            this.currentOffset = currentOffset;
        }

        public SnapshotBase Snapshot { get; }

        public Lexeme CurrentLexeme { get; private set; }

        public bool ReachedEnd { get; private set; }

        public bool TryGetNextLexeme(out Lexeme lexeme)
        {
            while (this.currentOffset < this.Snapshot.Length)
            {
                if (this.TryConsumeLexeme(out lexeme))
                {
                    this.CurrentLexeme = lexeme;
                    return true;
                }
                else
                {
                    // Keep us moving if we didn't find anything.
                    this.currentOffset++;
                }
            }

            // Ensure trailing semi-colon if there's no end-line.
            if (this.TryInsertSemicolon(out lexeme))
            {
                this.CurrentLexeme = lexeme;
                return true;
            }

            this.ReachedEnd = true;
            lexeme = default;
            return false;
        }

        private bool TryConsumeLexeme(out Lexeme lexeme)
        {
            switch (this.Snapshot[this.currentOffset])
            {
                case '+':
                    if (this.TryConsumeMultiCharacterSymbol('+', '+', out lexeme)) return true;
                    if (this.TryConsumeMultiCharacterSymbol('+', '=', out lexeme)) return true;
                    goto Symbol;
                case '-':
                    if (this.TryConsumeMultiCharacterSymbol('-', '-', out lexeme)) return true;
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
                case '.':
                case ':':
                Symbol:
                    lexeme = new Lexeme(new SnapshotSegment(this.Snapshot, this.currentOffset++, 1), LexemeType.Operator);
                    return true;
                case '"':
                    if (this.TryConsumeSpan(this.ConsumeStringLiteral, LexemeType.String, out lexeme)) return true;
                    break;
                case ';':
                    lexeme = new Lexeme(new SnapshotSegment(this.Snapshot, this.currentOffset++, 1), LexemeType.Semicolon);
                    return true;
                case '\r':
                case '\n':
                    // This is a bit awkward.. the semi-colon will be between the \r and \n
                    // but that shouldn't impact parsing.
                    if (this.TryInsertSemicolon(out lexeme)) return true;
                    break;
                case ' ':
                case '\t':
                case '\v':
                    break;
                default:
                    if (this.TryConsumeKeywordLiteralOrIdentifier(out lexeme)) return true;
                    break;
            }

            lexeme = default;
            return false;
        }

        private bool TryInsertSemicolon(out Lexeme lexeme)
        {
            if (this.TryPeekNext(out var next) && next == '\n')
            {
                this.currentOffset++;
            }

            switch (this.CurrentLexeme.Type)
            {
                case LexemeType.Keyword:
                    if (this.CurrentLexeme.Extent.Equals(Keywords.Break) ||
                        this.CurrentLexeme.Extent.Equals(Keywords.Continue) ||
                        this.CurrentLexeme.Extent.Equals(Keywords.FallThrough) ||
                        this.CurrentLexeme.Extent.Equals(Keywords.Return)) goto case LexemeType.Identifier;
                    break;
                case LexemeType.Operator:
                    if (this.CurrentLexeme.Extent.Equals("++") ||
                        this.CurrentLexeme.Extent.Equals("--") ||
                        this.CurrentLexeme.Extent.Equals(")") ||
                        this.CurrentLexeme.Extent.Equals("]") ||
                        this.CurrentLexeme.Extent.Equals("}")) goto case LexemeType.Identifier;
                        break;
                case LexemeType.Identifier:
                case LexemeType.Integer:
                case LexemeType.String:
                    lexeme = new Lexeme(new SnapshotSegment(this.Snapshot, this.currentOffset, 0), LexemeType.Semicolon);
                    return true;
            }

            lexeme = default;
            return false;
        }

        private bool TryConsumeKeywordLiteralOrIdentifier(out Lexeme lexeme)
        {
            if (char.IsDigit(this.Snapshot[this.currentOffset]))
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
            for (i = this.currentOffset; i < this.Snapshot.Length && char.IsDigit(this.Snapshot[i]); i++);

            var length = i - start;
            if (length > 0)
            {
                this.currentOffset += length;
                lexeme = new Lexeme(new SnapshotSegment(this.Snapshot, start, length), LexemeType.Integer);
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
                if (lexeme.Extent.Equals(keyword))
                {
                    return new Lexeme(lexeme.Extent, LexemeType.Keyword);
                }

                return lexeme;
            }

            switch (lexeme.Extent[0])
            {
                case 'b':
                    return LexemeForKeyword(Keywords.Break);
                case 'c':
                    if (lexeme.Extent.Equals(Keywords.Case) ||
                        lexeme.Extent.Equals(Keywords.Chan) ||
                        lexeme.Extent.Equals(Keywords.Const) ||
                        lexeme.Extent.Equals(Keywords.Continue))
                    {
                        return new Lexeme(lexeme.Extent, LexemeType.Keyword);
                    }
                    break;
                case 'd':
                    if (lexeme.Extent.Equals(Keywords.Default) ||
                        lexeme.Extent.Equals(Keywords.Defer))
                    {
                        return new Lexeme(lexeme.Extent, LexemeType.Keyword);
                    }
                    break;
                case 'e':
                    return LexemeForKeyword("else");
                case 'f':
                    if (lexeme.Extent.Equals(Keywords.FallThrough) ||
                        lexeme.Extent.Equals(Keywords.For) ||
                        lexeme.Extent.Equals(Keywords.Func))
                    {
                        return new Lexeme(lexeme.Extent, LexemeType.Keyword);
                    }
                    break;
                case 'g':
                    if (lexeme.Extent.Equals(Keywords.Go) ||
                        lexeme.Extent.Equals(Keywords.GoTo))
                    {
                        return new Lexeme(lexeme.Extent, LexemeType.Keyword);
                    }
                    break;
                case 'i':
                    if (lexeme.Extent.Equals(Keywords.If) ||
                        lexeme.Extent.Equals(Keywords.Import) ||
                        lexeme.Extent.Equals(Keywords.Interface))
                    {
                        return new Lexeme(lexeme.Extent, LexemeType.Keyword);
                    }
                    break;
                case 'm':
                    return LexemeForKeyword("map");
                case 'p':
                    return LexemeForKeyword("package");
                case 'r':
                    if (lexeme.Extent.Equals(Keywords.Range) ||
                        lexeme.Extent.Equals(Keywords.Return))
                    {
                        return new Lexeme(lexeme.Extent, LexemeType.Keyword);
                    }
                    break;
                case 's':
                    if (lexeme.Extent.Equals(Keywords.Select) ||
                        lexeme.Extent.Equals(Keywords.Struct) ||
                        lexeme.Extent.Equals(Keywords.Switch))
                    {
                        return new Lexeme(lexeme.Extent, LexemeType.Keyword);
                    }
                    break;
                case 't':
                    return LexemeForKeyword("type");
                case 'v':
                    return LexemeForKeyword("var");
            }

            return lexeme;
        }

        private bool TryConsumeMultiCharacterSymbol(char first, char second, out Lexeme lexeme)
        {
            if (this.Snapshot[this.currentOffset] == first &&
                this.TryPeekNext(out var next) &&
                next == second)
            {
                lexeme = new Lexeme(new SnapshotSegment(this.Snapshot, this.currentOffset, 2), LexemeType.Operator);
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
            if (!char.IsLetter(this.Snapshot[offset]))
            {
                return (0, 0);
            }

            int i = offset + 1;
            while (i < this.Snapshot.Length && char.IsLetterOrDigit(this.Snapshot[i]))
            {
                i++;
            }

            return (i - offset, 0);
        }

        private (int take, int skip) ConsumeStringLiteral(int offset)
        {
            if (this.Snapshot[offset] == '"')
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
            if (this.Snapshot[offset] == '*' &&
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

            while (this.currentOffset < this.Snapshot.Length)
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
                var segment = new SnapshotSegment(this.Snapshot, start, segmentLength);
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
            if (this.Snapshot[offset] == '\r')
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
            else if (this.Snapshot[offset] == '\n')
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
            if (this.currentOffset + 1 >= this.Snapshot.Length)
            {
                next = default;
                return false;
            }

            next = this.Snapshot[this.currentOffset + 1];
            return true;
        }
    }
}
