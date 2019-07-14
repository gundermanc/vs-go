namespace Go.CodeAnalysis.Lex
{
    /// <summary>
    /// Go lexeme types.
    /// </summary>
    public enum LexemeType
    {
        /// <summary>
        /// Single line comment.
        /// </summary>
        LineComment,

        /// <summary>
        /// Block comment.
        /// </summary>
        GeneralComment,

        /// <summary>
        /// Identifier.
        /// </summary>
        Identifier,

        /// <summary>
        /// Go keyword
        /// </summary>
        Keyword,

        /// <summary>
        /// String literal.
        /// </summary>
        String,

        /// <summary>
        /// Operator.
        /// </summary>
        Operator,

        /// <summary>
        /// Semicolon.
        /// </summary>
        Semicolon,

        /// <summary>
        /// Integer literal.
        /// </summary>
        Integer,

        /// <summary>
        /// Float literal.
        /// </summary>
        Float,
    }
}
