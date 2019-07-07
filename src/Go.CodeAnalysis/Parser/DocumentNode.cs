namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    public sealed class DocumentNode : ParseNode
    {
        public DocumentNode(SnapshotSegment extent, PackageDeclarationNode packageDeclaration) : base(extent, ImmutableArray<ParseNode>.Empty)
        {
            this.PackageDeclaration = packageDeclaration;
        }

        public PackageDeclarationNode PackageDeclaration { get; }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out DocumentNode parseNode)
        {
            if (!PackageDeclarationNode.TryParse(lexer, errors, out var packageDeclarationNode))
            {
                parseNode = null;
                return false;
            }

            parseNode = new DocumentNode(lexer.Snapshot.Extent, packageDeclarationNode);
            return true;
        }
    }
}
