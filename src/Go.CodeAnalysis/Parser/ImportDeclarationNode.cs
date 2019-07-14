namespace Go.CodeAnalysis.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Represents a single package import declaration.
    /// </summary>
    public class ImportDeclarationNode : ParseNode
    {
        public ImportDeclarationNode(SnapshotSegment code)
            : base(code, new ImmutableArray<ParseNode>())
        {
        }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out ImportDeclarationNode parseNode)
        {
            // to parse: alias/wildcard import
            // to parse: package name
            parseNode = new ImportDeclarationNode(lexer.CurrentLexeme.Extent);
            return true;
        }
    }
}
