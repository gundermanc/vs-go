namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    public sealed class DocumentNode : ParseNode
    {
        public DocumentNode(SnapshotSegment extent, PackageDeclarationNode packageDeclaration, DocumentBodyNode documentBodyNode)
            : base(extent, ImmutableArray.Create<ParseNode>(packageDeclaration, documentBodyNode))
        {
            this.PackageDeclaration = packageDeclaration;
            this.DocumentBody = documentBodyNode;
        }

        public PackageDeclarationNode PackageDeclaration { get; }

        public DocumentBodyNode DocumentBody { get; }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out DocumentNode parseNode)
        {
            if (!PackageDeclarationNode.TryParse(lexer, errors, out var packageDeclarationNode))
            {
                parseNode = null;
                return false;
            }

            DocumentBodyNode documentBodyNode = null;

            if (lexer.TryAdvanceLexeme(errors) && !DocumentBodyNode.TryParse(lexer, errors, out documentBodyNode))
            {
                parseNode = null;
                return false;
            }

            parseNode = new DocumentNode(lexer.Snapshot.Extent, packageDeclarationNode, documentBodyNode);
            return true;
        }
    }
}
