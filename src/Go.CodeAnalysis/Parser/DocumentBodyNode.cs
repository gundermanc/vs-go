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

            while (!lexer.ReachedEnd)
            {
                if (!lexer.IsCorrectLexemeTypeOrReportError(LexemeType.Keyword, errors) ||
                    !lexer.IsCorrectLexemeKeywordOrReportError(Keywords.Func, errors) ||
                    !FunctionDeclarationNode.TryParse(lexer, errors, out var funcNode))
                {
                    parseNode = null;
                    return false;
                }

                declarationsBuilder.Add(funcNode);
            }

            var extent = new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);
            parseNode = new DocumentBodyNode(extent, declarationsBuilder.ToImmutable());
            return true;
        }
    }
}
