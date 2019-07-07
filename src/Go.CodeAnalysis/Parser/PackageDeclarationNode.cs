namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    public sealed class PackageDeclarationNode : ParseNode
    {
        public PackageDeclarationNode(
            SnapshotSegment extent,
            SnapshotSegment packageNameExtent) : base(extent, ImmutableArray<ParseNode>.Empty)
        {
            this.PackageNameExtent = packageNameExtent;
        }

        public SnapshotSegment PackageNameExtent { get; }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out PackageDeclarationNode parseNode)
        {
            var start = lexer.CurrentLexeme.Segment.Start;

            if (!lexer.IsCorrectLexemeKeywordOrReportError(Keywords.Package, errors) ||
                !lexer.TryGetNextLexemeOrReportError(errors, out _))
            {
                parseNode = null;
                return false;
            }

            if (!lexer.IsCorrectLexemeTypeOrReportError(LexemeType.Identifier, errors))
            {
                parseNode = null;
                return false;
            }

            var statementExtent = new SnapshotSegment(lexer.CurrentLexeme.Segment.Snapshot, start, lexer.CurrentLexeme.Segment.End - start);
            parseNode = new PackageDeclarationNode(
                statementExtent,
                lexer.CurrentLexeme.Segment);

            return true;
        }
    }
}
