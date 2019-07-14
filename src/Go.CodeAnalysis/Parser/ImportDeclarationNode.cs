namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Represents a single package import declaration.
    /// </summary>
    public sealed class ImportDeclarationNode : ParseNode
    {
        public ImportDeclarationNode(
            SnapshotSegment code,
            string packageName,
            string packageAlias = null)
            : base(code, ImmutableArray<ParseNode>.Empty)
        {
            this.PackageName = packageName;
            this.PackageAlias = packageAlias;
        }

        public string PackageName { get; private set; }

        public string PackageAlias { get; private set; }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out ImportDeclarationNode parseNode)
        {
            // TODO: to add support for alias, wildcard import, and blank import.
            if (!lexer.IsCorrectLexemeTypeOrReportError(LexemeType.String, errors))
            {
                parseNode = null;
                return false;
            }

            parseNode = new ImportDeclarationNode(
                lexer.CurrentLexeme.Extent,
                lexer.CurrentLexeme.Extent.GetText());
            return true;
        }
    }
}
