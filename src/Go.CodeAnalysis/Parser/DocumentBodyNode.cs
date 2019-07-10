namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    public sealed class DocumentBodyNode : ParseNode
    {
        public DocumentBodyNode(SnapshotSegment extent, ImmutableArray<ParseNode> children) : base(extent, children)
        {
        }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out DocumentBodyNode parseNode)
        {
            var start = lexer.CurrentLexeme.Extent.Start;
            var declarationsBuilder = ImmutableArray.CreateBuilder<ParseNode>();

            // Check for keywords.
            if (!lexer.IsCorrectLexemeTypeOrReportError(LexemeType.Keyword, errors))
            {
                parseNode = null;
                return false;
            }

            // Loop through declarations.
            do
            {
                var keyword = lexer.CurrentLexeme.Extent.GetText();
                switch (keyword)
                {
                    case Keywords.Func:
                        if (!FunctionDeclarationNode.TryParse(lexer, errors, out var funcNode))
                        {
                            parseNode = null;
                            return false;
                        }
                        declarationsBuilder.Add(funcNode);
                        break;
                    default:
                        errors.Add(new Error(lexer.CurrentLexeme.Extent, string.Format(Strings.Error_UnexpectedKeyword, keyword, Keywords.Func)));
                        parseNode = null;
                        return false;
                }
            }
            while (lexer.TryAdvanceLexeme(errors));

            var extent = new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);
            parseNode = new DocumentBodyNode(extent, declarationsBuilder.ToImmutable());
            return true;
        }
    }
}
