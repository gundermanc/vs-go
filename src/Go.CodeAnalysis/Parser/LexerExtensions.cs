namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    internal static class LexerExtensions
    {
        public static bool TryAdvanceLexeme(this Lexer lexer)
        {
            while (lexer.TryGetNextLexeme(out var lexeme))
            {
                if (lexeme.Type != LexemeType.GeneralComment &&
                    lexeme.Type != LexemeType.LineComment)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryAdvanceLexemeOrReportError(this Lexer lexer, IList<Error> errors)
        {
            if (lexer.TryAdvanceLexeme())
            {
                return true;
            }

            // Prevent out of range exception for empty files.
            var errorSpan = lexer.Snapshot.Length > 0 ?
                new SnapshotSegment(lexer.Snapshot, lexer.Snapshot.Extent.End - 1, 1) :
                new SnapshotSegment(lexer.Snapshot, 0, 0);

            errors.Add(new Error(errorSpan, Strings.Error_EndOfFile));
            lexer.TryConsumeToEndOfStatementOrReportError(errors);
            return false;
        }

        // TODO: support multi-character operators
        public static bool IsCorrectLexemeOperatorOrReportError(this Lexer lexer, char op, IList<Error> errors)
        {
            if (!lexer.IsCorrectLexemeTypeOrReportError(LexemeType.Operator, errors))
            {
                return false;
            }

            if (!lexer.CurrentLexeme.Extent.TryGetSingleChar(out var c) && c == op)
            {
                var message = string.Format(Strings.Error_UnexpectedKeyword, lexer.CurrentLexeme.Extent.GetText(), op);
                errors.Add(new Error(lexer.CurrentLexeme.Extent, message));
                return false;
            }

            return true;
        }

        public static bool IsCorrectLexemeKeywordOrReportError(this Lexer lexer, string keyword, IList<Error> errors)
        {
            if (!lexer.IsCorrectLexemeTypeOrReportError(LexemeType.Keyword, errors))
            {
                return false;
            }

            if (!lexer.CurrentLexeme.Extent.Equals(keyword))
            {
                var message = string.Format(Strings.Error_UnexpectedKeyword, lexer.CurrentLexeme.Extent.GetText(), keyword);
                errors.Add(new Error(lexer.CurrentLexeme.Extent, message));
                return false;
            }

            return true;
        }

        public static bool IsCorrectLexemeTypeOrReportError(this Lexer lexer, LexemeType type, IList<Error> errors)
        {
            if (lexer.CurrentLexeme.Type != type)
            {
                var message = string.Format(Strings.Error_UnexpectedLexmeTypeFormatString, lexer.CurrentLexeme.Type, type);
                errors.Add(new Error(lexer.CurrentLexeme.Extent, message));
                lexer.TryConsumeToEndOfStatementOrReportError(errors);
                return false;
            }

            return true;
        }

        public static bool TryConsumeToEndOfStatementOrReportError(this Lexer lexer, IList<Error> errors)
        {
            while (lexer.CurrentLexeme.Type != LexemeType.Semicolon && lexer.TryGetNextLexeme(out _)) { }

            if (lexer.CurrentLexeme.Type != LexemeType.Semicolon)
            {
                var message = string.Format(Strings.Error_UnexpectedLexmeTypeFormatString, lexer.CurrentLexeme.Type, LexemeType.Semicolon);
                errors.Add(new Error(lexer.CurrentLexeme.Extent, message));
                return false;
            }

            return true;
        }
    }
}
