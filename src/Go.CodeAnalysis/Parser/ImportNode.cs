namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Represents a single import line or import block.
    /// </summary>
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

            // Parse "import" keyword.
            if (!lexer.IsCorrectLexemeKeywordOrReportError(Keywords.Import, errors)
                || !lexer.TryAdvanceLexemeOrReportError(errors))
            {
                parseNode = null;
                return false;
            }

            // Single line import
            if (!lexer.IsCorrectLexemeOperatorOrReportError('(', errors))
            {
                var extent = new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);
                parseNode = new ImportNode(extent, lexer.CurrentLexeme.Extent);
                return true;
            }

            // Block import
            parseNode = null;
            return false;
        }
    }
}
