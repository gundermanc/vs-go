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
    public class ImportDeclarationNode : ParseNode
    {
        public ImportDeclarationNode(
            SnapshotSegment code,
            string packageName,
            string alias = null)
            : base(code, ImmutableArray<ParseNode>.Empty)
        {
        }

        public string PackageName { get; private set; }

        public string PackageAlias { get; private set; }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out ImportDeclarationNode parseNode)
        {
            // to parse: alias/wildcard import
            // to parse: package name
            parseNode = new ImportDeclarationNode(lexer.CurrentLexeme.Extent, string.Empty, string.Empty);
            return true;
        }
    }
}
