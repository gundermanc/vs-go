namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    public sealed class BlockNode : ParseNode
    {
        public BlockNode(SnapshotSegment extent) : base(extent, ImmutableArray<ParseNode>.Empty)
        {
        }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out BlockNode parseNode)
        {
            var start = lexer.CurrentLexeme.Extent.Start;
            if (!lexer.IsCorrectLexemeOperatorOrReportError('{', errors))
            {
                parseNode = null;
                return false;
            }

            // TODO: parse statements.

            if (!lexer.TryGetNextLexemeOrReportError(errors, out _) ||
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
