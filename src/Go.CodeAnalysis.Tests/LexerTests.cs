namespace Go.CodeAnalysis.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        [Description("Ensure we get nothing back from the empty string")]
        public void Lexer_TryGetNext_Empty_NoLexemes()
        {
            var lexer = Lexer.Create(new StringSnapshot(string.Empty));

            Assert.IsFalse(lexer.TryGetNextLexeme(out var lexeme));
        }

        [TestMethod]
        [Description("Ensure we get nothing back from blank lines")]
        public void Lexer_TryGetNext_BlankLines_NoLexemes()
        {
            var lexer = Lexer.Create(new StringSnapshot(string.Empty));

            Assert.IsFalse(lexer.TryGetNextLexeme(out var lexeme));
        }

        [TestMethod]
        [Description("Ensure we can process a leading line comment")]
        public void Lexer_TryGetNext_LeadingLineComment_CommentLexeme()
        {
            var lexer = Lexer.Create(new StringSnapshot("// My comment"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(0, lexeme.Segment.Start);
            Assert.AreEqual(13, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.LineComment, lexeme.Type);
        }

        [TestMethod]
        [Description("Ensure we can process a line comment with whitespace")]
        public void Lexer_TryGetNext_LineCommentInWhitespace_CommentLexeme()
        {
            var lexer = Lexer.Create(new StringSnapshot("      // My comment\r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(6, lexeme.Segment.Start);
            Assert.AreEqual(13, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.LineComment, lexeme.Type);
        }

        /// <summary>
        /// Known issue where we're not handling non-Windows line endings right.
        /// </summary>
        [TestMethod]
        [Description("Ensure we can process a line comment with whitespace")]
        public void Lexer_TryGetNext_LineCommentWithMacLineEnding_CommentLexeme()
        {
            var lexer = Lexer.Create(new StringSnapshot("      // My comment\r  "));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(6, lexeme.Segment.Start);
            Assert.AreEqual(13, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.LineComment, lexeme.Type);
        }

        [TestMethod]
        [Description("Ensure we can process a general comment with whitespace")]
        public void Lexer_TryGetNext_GeneralCommentInWhitespace_CommentLexeme()
        {
            var lexer = Lexer.Create(new StringSnapshot("    /* My comment */  \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(4, lexeme.Segment.Start);
            Assert.AreEqual(16, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.GeneralComment, lexeme.Type);
        }

        [TestMethod]
        [Description("Ensure we can process an identifier")]
        public void Lexer_TryGetNext_IdentifierInWhitespace_IdentiferLexeme()
        {
            var lexer = Lexer.Create(new StringSnapshot("    MyNamed Th1ing  \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(4, lexeme.Segment.Start);
            Assert.AreEqual(7, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.Identifier, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(12, lexeme.Segment.Start);
            Assert.AreEqual(6, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.Identifier, lexeme.Type);

            Assert.IsFalse(lexer.TryGetNextLexeme(out _));
        }

        [TestMethod]
        [Description("Ensure we can process keywords")]
        public void Lexer_TryGetNext_Keywords()
        {
            var lexer = Lexer.Create(new StringSnapshot("break case chan const continue default defer else fallthrough for func go goto if import interface map package range return select struct switch type var"));

            // break
            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(0, lexeme.Segment.Start);
            Assert.AreEqual(5, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // case
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(6, lexeme.Segment.Start);
            Assert.AreEqual(4, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // chan
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(11, lexeme.Segment.Start);
            Assert.AreEqual(4, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // const
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(16, lexeme.Segment.Start);
            Assert.AreEqual(5, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // continue
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(22, lexeme.Segment.Start);
            Assert.AreEqual(8, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // default
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(31, lexeme.Segment.Start);
            Assert.AreEqual(7, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // defer
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(39, lexeme.Segment.Start);
            Assert.AreEqual(5, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // else
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // fallthrough
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // for
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // func
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // go
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // goto
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // if
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // import
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // interface
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // map
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // package
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // range
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // return
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // select
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // struct
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // switch
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // type
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // var
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);
        }

        [TestMethod]
        [Description("Ensure we can process string literals")]
        public void Lexer_TryGetNext_StringLiterals()
        {
            var lexer = Lexer.Create(new StringSnapshot("    \"Foo\"  "));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(4, lexeme.Segment.Start);
            Assert.AreEqual(5, lexeme.Segment.Length);
            Assert.AreEqual(LexemeType.String, lexeme.Type);
        }
    }
}
