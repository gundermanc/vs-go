namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    public sealed class DocumentNode : ParseNode
    {
        public DocumentNode(
            SnapshotSegment extent,
            PackageDeclarationNode packageDeclaration,
            ImportsNode importsNode,
            DocumentBodyNode documentBodyNode)
            : base(extent, ImmutableArray.Create<ParseNode>(packageDeclaration, documentBodyNode, importsNode))
        {
            this.PackageDeclaration = packageDeclaration;
            this.ImportsNode = importsNode;
            this.DocumentBody = documentBodyNode;
        }

        public PackageDeclarationNode PackageDeclaration { get; }

        public ImportsNode ImportsNode { get; }

        public DocumentBodyNode DocumentBody { get; }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out DocumentNode parseNode)
        {
            if (!PackageDeclarationNode.TryParse(lexer, errors, out var packageDeclarationNode))
            {
                parseNode = null;
                return false;
            }

            ImportsNode importsNode = null;
            DocumentBodyNode documentBodyNode = null;

            if (lexer.TryAdvanceLexeme())
            {
                // Parse import node.
                if (lexer.CurrentLexeme.Type == LexemeType.Keyword &&
                    lexer.CurrentLexeme.Extent.Equals(Keywords.Import) &&
                    !ImportsNode.TryParse(lexer, errors, out importsNode))
                {
                    parseNode = null;
                    return false;
                }

                if (!DocumentBodyNode.TryParse(lexer, errors, out documentBodyNode))
                {
                    parseNode = null;
                    return false;
                }
            }

            parseNode = new DocumentNode(
                lexer.Snapshot.Extent,
                packageDeclarationNode,
                importsNode,
                documentBodyNode);
            return true;
        }
    }
}
