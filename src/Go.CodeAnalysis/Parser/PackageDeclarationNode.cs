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
            var start = lexer.CurrentLexeme.Extent.Start;

            if (!lexer.IsCorrectLexemeKeywordOrReportError(Keywords.Package, errors) ||
                !lexer.TryAdvanceLexemeOrReportError(errors))
            {
                parseNode = null;
                return false;
            }

            if (!lexer.IsCorrectLexemeTypeOrReportError(LexemeType.Identifier, errors))
            {
                parseNode = null;
                return false;
            }

            var packageNameExtent = lexer.CurrentLexeme.Extent;
            var statementExtent = new SnapshotSegment(lexer.CurrentLexeme.Extent.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);

            if (!lexer.TryAdvanceLexemeOrReportError(errors) ||
                !lexer.IsCorrectLexemeTypeOrReportError(LexemeType.Semicolon, errors))
            {
                parseNode = null;
                return false;
            }

            parseNode = new PackageDeclarationNode(
                statementExtent,
                packageNameExtent);

            return true;
        }
    }
}
