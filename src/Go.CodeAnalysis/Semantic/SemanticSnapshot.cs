namespace Go.CodeAnalysis.Semantic
{
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Parser;

    public sealed class SemanticSnapshot
    {
        public static SemanticSnapshot Create(ParseSnapshot parseSnapshot)
        {
            var semanticVisitor = new SemanticVisitor(parseSnapshot);

            var rootNode = semanticVisitor.CreateSemanticTree();

            return new SemanticSnapshot(rootNode.Item1, rootNode.Item2.AddRange(parseSnapshot.Errors));
        }

        private SemanticSnapshot(DocumentSemanticNode rootNode, ImmutableArray<Error> errors)
        {
            this.RootNode = rootNode;
            this.Errors = errors;
        }

        public ImmutableArray<Error> Errors { get; }

        public DocumentNode RootNode { get; }
    }
}
