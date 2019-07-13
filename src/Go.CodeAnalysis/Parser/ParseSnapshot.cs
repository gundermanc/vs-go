namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    public sealed class ParseSnapshot
    {
        public ImmutableArray<Error> Errors { get; }

        public DocumentNode RootNode { get; }

        public static ParseSnapshot Create(SnapshotBase snapshotBase)
        {
            var errorsBuilder = ImmutableArray.CreateBuilder<Error>();

            var lexer = Lexer.Create(snapshotBase);

            if (lexer.TryAdvanceLexemeOrReportError(errorsBuilder) &&
                DocumentNode.TryParse(lexer, errorsBuilder, out var rootNode))
            {
                return new ParseSnapshot(rootNode, errorsBuilder.ToImmutable());
            }
            else
            {
                return new ParseSnapshot(
                    new DocumentNode(
                        snapshotBase.Extent,
                        packageDeclaration: null,
                        importsNode: null,
                        documentBodyNode: null),
                    errorsBuilder.ToImmutable());
            }
        }

        private ParseSnapshot(DocumentNode rootNode, ImmutableArray<Error> errors)
        {
            this.Errors = errors;
            this.RootNode = rootNode;
        }
    }
}
