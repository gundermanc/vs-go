namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Represents a single package import declaration.
    /// </summary>
    public sealed class ImportDeclarationNode : ParseNodeBase
    {
        public ImportDeclarationNode(
            SnapshotSegment code,
            SnapshotSegment packageName)
            : base(code)
        {
            this.PackageName = packageName;
        }

        public SnapshotSegment PackageName { get; }

        public SnapshotSegment PackageAlias { get; }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out ImportDeclarationNode parseNode)
        {
            // TODO: to add support for alias, wildcard import, and blank import.
            if (!lexer.IsCorrectLexemeTypeOrReportError(LexemeType.String, errors))
            {
                parseNode = null;
                return false;
            }

            parseNode = new ImportDeclarationNode(lexer.CurrentLexeme.Extent, lexer.CurrentLexeme.Extent);
            return true;
        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
