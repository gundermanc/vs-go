namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    public sealed class DocumentBodyNode : ParseNodeBase
    {
        public DocumentBodyNode(SnapshotSegment extent, ImmutableArray<ParseNodeBase> declarations) : base(extent)
        {
            this.Declarations = declarations;
        }

        public ImmutableArray<ParseNodeBase> Declarations { get; }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);

        public static bool TryParse(Lexer lexer, IList<Error> errors, out DocumentBodyNode parseNode)
        {
            var start = lexer.CurrentLexeme.Extent.Start;
            var declarationsBuilder = ImmutableArray.CreateBuilder<ParseNodeBase>();

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
