namespace Go.CodeAnalysis.Editor
{
    using System;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Parser;

    internal sealed class FunctionAggregationVisitor : DepthFirstTraversalVisitorBase
    {
        private readonly ParseSnapshot parseSnapshot;
        private readonly ImmutableArray<FunctionDeclarationNode>.Builder builder
            = ImmutableArray.CreateBuilder<FunctionDeclarationNode>();

        public FunctionAggregationVisitor(ParseSnapshot parseSnapshot)
        {
            this.parseSnapshot = parseSnapshot
                ?? throw new ArgumentNullException(nameof(parseSnapshot));
        }

        public override void Visit(DocumentNode parseNode) => parseNode?.DocumentBody?.Accept(this);

        public override void Visit(FunctionDeclarationNode parseNode)
        {
            this.builder.Add(parseNode);
            base.Visit(parseNode);
        }

        public ImmutableArray<FunctionDeclarationNode> GetFunctions()
        {
            this.parseSnapshot.RootNode.Accept(this);

            return this.builder.ToImmutable();
        }
    }
}
