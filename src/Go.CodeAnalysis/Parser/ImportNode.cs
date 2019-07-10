using System.Collections.Generic;
using System.Collections.Immutable;
using Go.CodeAnalysis.Common;
using Go.CodeAnalysis.Lex;
using Go.CodeAnalysis.Text;

namespace Go.CodeAnalysis.Parser
{
    public sealed class ImportNode : ParseNode
    {
        public ImportNode(SnapshotSegment extent, SnapshotSegment importExtent) : base(extent, ImmutableArray<ParseNode>.Empty)
        {
            this.ImportExtent = importExtent;
        }

        public SnapshotSegment ImportExtent { get; }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out ImportNode parseNode)
        {
            var start = lexer.CurrentLexeme.Extent.Start;

            if (!lexer.IsCorrectLexemeKeywordOrReportError(Keywords.Import, errors) ||
                !lexer.TryAdvanceLexemeOrReportError(errors) ||
                !lexer.IsCorrectLexemeTypeOrReportError(LexemeType.String, errors))
            {
                parseNode = null;
                return false;
            }

            var extent = new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);
            parseNode = new ImportNode(extent, lexer.CurrentLexeme.Extent);
            return true;
        }
    }
}
