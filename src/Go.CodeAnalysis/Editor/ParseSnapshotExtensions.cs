namespace Go.CodeAnalysis.Editor
{
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Parser;

    public static class ParseSnapshotExtensions
    {
        public static ImmutableArray<FunctionDeclarationNode> GetFunctions(this ParseSnapshot parseSnapshot)
        {
            var aggregator = new FunctionAggregationVisitor(parseSnapshot);

            return aggregator.GetFunctions();
        }

        public static bool TryGetDeepestNode<TNode>(this ParseSnapshot parseSnapshot, int position, out TNode node) where TNode : ParseNodeBase
        {
            var pointSearch = new PointSearchVisitor(parseSnapshot, position, candidate => candidate is TNode);

            var discoveredNode = pointSearch.FindNode();
            if (discoveredNode is TNode concreteDiscoveredNode)
            {
                node = concreteDiscoveredNode;
                return true;
            }
            else
            {
                node = null;
                return false;
            }
        }
    }
}
