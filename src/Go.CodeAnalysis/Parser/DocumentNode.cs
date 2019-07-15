namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Represents a parsed Go source code file. It is the root of an abstract syntax tree.
    /// </summary>
    public class DocumentNode : ParseNodeBase
    {
        public DocumentNode(
            SnapshotSegment extent,
            PackageDeclarationNode packageDeclaration,
            ImportsNode importsNode,
            DocumentBodyNode documentBodyNode) : base(extent)
        {
            this.PackageDeclaration = packageDeclaration;
            this.ImportsNode = importsNode;
            this.DocumentBody = documentBodyNode;
        }

        /// <summary>
        /// Package declaration node.
        /// </summary>
        public PackageDeclarationNode PackageDeclaration { get; }

        /// <summary>
        /// Import node.
        /// </summary>
        public ImportsNode ImportsNode { get; }

        /// <summary>
        /// Source code body node.
        /// </summary>
        public DocumentBodyNode DocumentBody { get; }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);

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
