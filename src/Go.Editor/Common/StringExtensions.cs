namespace Go.Editor.Common
{
    using System.Collections.Generic;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text.Adornments;

    internal static class StringExtensions
    {
        public static ClassifiedTextElement ToClassifiedTextElement(this string text)
        {
            var runs = new List<ClassifiedTextRun>();
            var lexer = Lexer.Create(new StringSnapshot(text));
            while (lexer.TryGetNextLexeme(out var lexeme))
            {
                runs.Add(new ClassifiedTextRun(lexeme.Type.ToClassificationTypeName(), lexeme.Extent.GetText()));

                // Hack: add space in after each item since lexer swallows whitespace.
                runs.Add(new ClassifiedTextRun(PredefinedClassificationTypeNames.Other, " "));
            }

            return new ClassifiedTextElement(runs);
        }

        public static ClassifiedTextElement ToPlainTextClassifiedTextElement(this string text)
        {
            return new ClassifiedTextElement(new ClassifiedTextRun(PredefinedClassificationTypeNames.Other, text));
        }
    }
}
