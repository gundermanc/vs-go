namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    internal static class LexerExtensions
    {
        public static bool TryGetNextLexemeOrReportError(this Lexer lexer, IList<Error> errors, out Lexeme lexeme)
        {
            if (!lexer.TryGetNextLexeme(out lexeme))
            {
                // Prevent out of range exception for empty files.
                var errorSpan = lexer.Snapshot.Length > 0 ?
                    new SnapshotSegment(lexer.Snapshot, lexer.Snapshot.Extent.End - 1, 1) :
                    new SnapshotSegment(lexer.Snapshot, 0, 0);

                errors.Add(new Error(errorSpan, Strings.Error_EndOfFile));
                lexer.TryConsumeToEndOfStatementOrReportError(errors);
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

            if (!lexer.CurrentLexeme.Segment.Equals(keyword))
            {
                var message = string.Format(Strings.Error_UnexpectedKeyword, lexer.CurrentLexeme.Segment.GetText(), keyword);
                errors.Add(new Error(lexer.CurrentLexeme.Segment, message));
                return false;
            }

            return true;
        }

        public static bool IsCorrectLexemeTypeOrReportError(this Lexer lexer, LexemeType type, IList<Error> errors)
        {
            if (lexer.CurrentLexeme.Type != type)
            {
                var message = string.Format(Strings.Error_UnexpectedLexmeTypeFormatString, lexer.CurrentLexeme.Type, type);
                errors.Add(new Error(lexer.CurrentLexeme.Segment, message));
                lexer.TryConsumeToEndOfStatementOrReportError(errors);
                return false;
            }

            return true;
        }

        public static bool TryConsumeToEndOfStatementOrReportError(this Lexer lexer, IList<Error> errors)
        {
            while (lexer.CurrentLexeme.Type != LexemeType.Semicolon && lexer.TryGetNextLexeme(out _));

            if (lexer.CurrentLexeme.Type != LexemeType.Semicolon)
            {
                var message = string.Format(Strings.Error_UnexpectedLexmeTypeFormatString, lexer.CurrentLexeme.Type, LexemeType.Semicolon);
                errors.Add(new Error(new SnapshotSegment(), message));
                return false;
            }

            return true;
        }
    }
}
