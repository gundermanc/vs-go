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
        }

        [TestMethod]
        [Description("Ensure we can process a line comment with whitespace")]
        public void Lexer_TryGetNext_LineCommentInWhitespace_CommentLexeme()
        {
            var lexer = Lexer.Create(new StringSnapshot("      // My comment\r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(6, lexeme.Segment.Start);
            Assert.AreEqual(13, lexeme.Segment.Length);
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
        }

        [TestMethod]
        [Description("Ensure we can process a general comment with whitespace")]
        public void Lexer_TryGetNext_GeneralCommentInWhitespace_CommentLexeme()
        {
            var lexer = Lexer.Create(new StringSnapshot("    /* My comment */  \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(4, lexeme.Segment.Start);
            Assert.AreEqual(16, lexeme.Segment.Length);
        }
    }
}
