namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    public class FunctionDeclarationNode : ParseNodeBase
    {
        public FunctionDeclarationNode(SnapshotSegment extent, SnapshotSegment functionNameExtent, BlockNode blockNode)
            : base(extent)
        {
            this.FunctionNameExtent = functionNameExtent;
            this.BlockNode = blockNode;
        }

        public SnapshotSegment FunctionNameExtent { get; }

        public BlockNode BlockNode { get; }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);

        public static bool TryParse(Lexer lexer, IList<Error> errors, out FunctionDeclarationNode parseNode)
        {
            var start = lexer.CurrentLexeme.Extent.Start;

            if (!lexer.IsCorrectLexemeKeywordOrReportError(Keywords.Func, errors) ||
                !lexer.TryAdvanceLexemeOrReportError(errors) ||
                !lexer.IsCorrectLexemeTypeOrReportError(LexemeType.Identifier, errors))
            {
                parseNode = null;
                return false;
            }

            var functionNameExtent = lexer.CurrentLexeme.Extent;

            // TODO: parse function arguments.
            if (!lexer.TryAdvanceLexemeOrReportError(errors) ||
                !lexer.IsCorrectLexemeOperatorOrReportError('(', errors) ||
                !lexer.TryAdvanceLexemeOrReportError(errors) ||
                !lexer.IsCorrectLexemeOperatorOrReportError(')', errors) ||
                !lexer.TryAdvanceLexemeOrReportError(errors))
            {
                parseNode = null;
                return false;
            }

            if (!BlockNode.TryParse(lexer, errors, out var blockNode))
            {
                parseNode = null;
                return false;
            }

            var extent = new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);

            if (!lexer.TryAdvanceLexemeOrReportError(errors) ||
                !lexer.IsCorrectLexemeTypeOrReportError(LexemeType.Semicolon, errors))
            {
                parseNode = null;
                return false;
            }

            parseNode = new FunctionDeclarationNode(extent, functionNameExtent, blockNode);

            lexer.TryAdvanceLexeme();

            return true;
        }
    }
}
