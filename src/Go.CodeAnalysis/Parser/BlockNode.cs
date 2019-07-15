namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Represents a parsed code block.
    /// </summary>
    public class BlockNode : ParseNodeBase
    {
        public BlockNode(SnapshotSegment extent) : base(extent)
        {
        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);

        public static bool TryParse(Lexer lexer, IList<Error> errors, out BlockNode parseNode)
        {
            var start = lexer.CurrentLexeme.Extent.Start;
            if (!lexer.IsCorrectLexemeOperatorOrReportError('{', errors))
            {
                parseNode = null;
                return false;
            }

            // TODO: parse statements.

            if (!lexer.TryAdvanceLexemeOrReportError(errors) ||
                !lexer.IsCorrectLexemeOperatorOrReportError('}', errors))
            {
                parseNode = null;
                return false;
            }

            parseNode = new BlockNode(new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start));
            return true;
        }
    }
}
