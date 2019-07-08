namespace Go.CodeAnalysis.Tests.Lex
{
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LexerTests
    {
        // TODO: missing bits of spec https://golang.org/ref/spec#Lexical_elements
        // - add support for string escape sequences.
        // - add support for float literals
        // - add support for imaginary literals

        [TestMethod]
        [Description("Ensure we get nothing back from the empty string")]
        public void Lexer_TryGetNext_Empty_NoLexemes()
        {
            var lexer = Lexer.Create(new StringSnapshot(string.Empty));
            Assert.IsFalse(lexer.TryGetNextLexeme(out _));
        }

        [TestMethod]
        [Description("Ensure we get nothing back from blank lines")]
        public void Lexer_TryGetNext_BlankLines_NoLexemes()
        {
            var lexer = Lexer.Create(new StringSnapshot(string.Empty));
            Assert.IsFalse(lexer.TryGetNextLexeme(out _));
        }

        [TestMethod]
        [Description("Ensure we can process a leading line comment")]
        public void Lexer_TryGetNext_LeadingLineComment_CommentLexeme()
        {
            var lexer = Lexer.Create(new StringSnapshot("// My comment"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(0, lexeme.Extent.Start);
            Assert.AreEqual(13, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.LineComment, lexeme.Type);
        }

        [TestMethod]
        [Description("Ensure we can process a line comment with whitespace")]
        public void Lexer_TryGetNext_LineCommentInWhitespace_CommentLexeme()
        {
            var lexer = Lexer.Create(new StringSnapshot("      // My comment\r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(6, lexeme.Extent.Start);
            Assert.AreEqual(13, lexeme.Extent.Length);
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
            Assert.AreEqual(6, lexeme.Extent.Start);
            Assert.AreEqual(13, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.LineComment, lexeme.Type);
        }

        [TestMethod]
        [Description("Ensure we can process a general comment with whitespace")]
        public void Lexer_TryGetNext_GeneralCommentInWhitespace_CommentLexeme()
        {
            var lexer = Lexer.Create(new StringSnapshot("    /* My comment */  \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(4, lexeme.Extent.Start);
            Assert.AreEqual(16, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.GeneralComment, lexeme.Type);
        }

        [TestMethod]
        [Description("Ensure we can process an identifier")]
        public void Lexer_TryGetNext_IdentifierInWhitespace_IdentiferLexeme()
        {
            var lexer = Lexer.Create(new StringSnapshot("    MyNamed Th1ing  \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(4, lexeme.Extent.Start);
            Assert.AreEqual(7, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Identifier, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(12, lexeme.Extent.Start);
            Assert.AreEqual(6, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Identifier, lexeme.Type);
        }

        [TestMethod]
        [Description("Ensure we can process keywords")]
        public void Lexer_TryGetNext_Keywords()
        {
            var lexer = Lexer.Create(new StringSnapshot("break case chan const continue default defer else fallthrough for func go goto if import interface map package range return select struct switch type var"));

            // break
            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(0, lexeme.Extent.Start);
            Assert.AreEqual(5, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // case
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(6, lexeme.Extent.Start);
            Assert.AreEqual(4, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // chan
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(11, lexeme.Extent.Start);
            Assert.AreEqual(4, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // const
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(16, lexeme.Extent.Start);
            Assert.AreEqual(5, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // continue
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(22, lexeme.Extent.Start);
            Assert.AreEqual(8, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // default
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(31, lexeme.Extent.Start);
            Assert.AreEqual(7, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            // defer
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(39, lexeme.Extent.Start);
            Assert.AreEqual(5, lexeme.Extent.Length);
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
            Assert.AreEqual(4, lexeme.Extent.Start);
            Assert.AreEqual(5, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.String, lexeme.Type);
        }

        [TestMethod]
        [Description("Ensure we can process single character operators")]
        public void Lexer_TryGetNext_SingleCharOperators()
        {
            var lexer = Lexer.Create(new StringSnapshot(" + - * / % & | ^ < > = ! ( ) [ ] { } , ; . :"));

            // +
            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(1, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // -
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(3, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // *
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(5, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // /
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(7, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // /
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(9, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // &
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(11, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // |
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(13, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // ^
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(15, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // <
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(17, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // >
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(19, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // =
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(21, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // !
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(23, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // (
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(25, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // )
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(27, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // [
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(29, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // ]
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(31, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // {
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(33, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // }
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(35, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // ,
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(37, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // ;
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(39, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);

            // .
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(41, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // :
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(43, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);
        }

        [TestMethod]
        [Description("Ensure we can process multi character operators")]
        public void Lexer_TryGetNext_MultiCharOperators()
        {
            var lexer = Lexer.Create(new StringSnapshot(" << >> &^ += -= *= /= %= &= |= ^="));

            // <<
            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(1, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // >>
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(4, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // &^
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(7, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // +=
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(10, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // -=
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(13, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // *=
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(16, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // /=
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(19, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // %=
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(22, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // &=
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(25, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // |=
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(28, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            // ^=
            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(31, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);
        }

        [TestMethod]
        [Description("Ensure we can process multi character operators")]
        public void Lexer_TryGetNext_IntegerLiteral()
        {
            var lexer = Lexer.Create(new StringSnapshot("  1234567890 "));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(10, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Integer, lexeme.Type);

            Assert.IsFalse(lexer.TryGetNextLexeme(out _));
        }

        [TestMethod]
        [Description("Verify correct placement of implicit semi-colons in token stream")]
        public void Lexer_TryGetNext_ImplicitSemicolonPlacement()
        {
            // From: https://golang.org/ref/spec#Semicolons
            // When the input is broken into tokens, a semicolon is automatically inserted
            // into the token stream immediately after a line's final token if that token is
            // - an identifier
            // - an integer, floating-point, imaginary, rune, or string literal
            // - one of the keywords break, continue, fallthrough, or return
            // - one of the operators and punctuation ++, --, ), ], or }

            // TODO: support for floating-point, imaginary, rune.


            // - an identifier
            var lexer = Lexer.Create(new StringSnapshot("  foobar \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(6, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Identifier, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(10, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);


            // - an integer
            lexer = Lexer.Create(new StringSnapshot("  123456 \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(6, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Integer, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(10, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);


            // - a string literal
            lexer = Lexer.Create(new StringSnapshot("  \"hello\" \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(7, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.String, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(11, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);


            // - break
            lexer = Lexer.Create(new StringSnapshot("  break \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(5, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(9, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);


            // - continue
            lexer = Lexer.Create(new StringSnapshot("  continue \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(8, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(12, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);


            // - fallthrough
            lexer = Lexer.Create(new StringSnapshot("  fallthrough \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(11, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(15, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);


            // - return
            lexer = Lexer.Create(new StringSnapshot("  return \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(6, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Keyword, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(10, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);


            // - operator ++
            lexer = Lexer.Create(new StringSnapshot("  ++ \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(6, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);


            // - operator --
            lexer = Lexer.Create(new StringSnapshot("  -- \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(2, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(6, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);


            // - operator )
            lexer = Lexer.Create(new StringSnapshot("  ) \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(5, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);

            // - operator ]
            lexer = Lexer.Create(new StringSnapshot("  ] \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(5, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);

            // - operator ]
            lexer = Lexer.Create(new StringSnapshot("  } \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Operator, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(5, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);
        }

        [TestMethod]
        [Description("Verify correct placement of explicit semi-colons in token stream")]
        public void Lexer_TryGetNext_ExplicitSemicolonPlacement()
        {
            // - an identifier
            var lexer = Lexer.Create(new StringSnapshot("  foobar; \r\n"));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(6, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Identifier, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(8, lexeme.Extent.Start);
            Assert.AreEqual(1, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);
        }

        [TestMethod]
        [Description("Verify correct placement of implicit semi-colons in token stream without a trailing end-line")]
        public void Lexer_TryGetNext_ImplicitSemicolonPlacementWithoutEndLine()
        {
            // From: https://golang.org/ref/spec#Semicolons

            // - an identifier
            var lexer = Lexer.Create(new StringSnapshot("  foobar "));

            Assert.IsTrue(lexer.TryGetNextLexeme(out var lexeme));
            Assert.AreEqual(2, lexeme.Extent.Start);
            Assert.AreEqual(6, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Identifier, lexeme.Type);

            Assert.IsTrue(lexer.TryGetNextLexeme(out lexeme));
            Assert.AreEqual(9, lexeme.Extent.Start);
            Assert.AreEqual(0, lexeme.Extent.Length);
            Assert.AreEqual(LexemeType.Semicolon, lexeme.Type);
        }
    }
}
