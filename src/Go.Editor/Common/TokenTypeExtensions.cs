namespace Go.Editor.Common
{
    using Go.Interop;
    using Microsoft.VisualStudio.Language.StandardClassification;

    public static class TokenTypeExtensions
    {
        public static string ToClassificationTypeName(this TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.KEYWORD:
                    return PredefinedClassificationTypeNames.Keyword;
                case TokenType.IDENTIFIER:
                    return PredefinedClassificationTypeNames.Identifier;
                case TokenType.STRING:
                    return PredefinedClassificationTypeNames.String;
                case TokenType.RESOLVEDTYPE:
                    return PredefinedClassificationTypeNames.Type;
                case TokenType.LITERAL:
                    return PredefinedClassificationTypeNames.Literal;
                case TokenType.COMMENT:
                    return PredefinedClassificationTypeNames.Comment;
                default:
                    return "text";
            }
        }
    }
}
