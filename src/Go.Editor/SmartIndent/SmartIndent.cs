namespace Go.Editor.SmartIndent
{
    using System;
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Editor;
    using Go.CodeAnalysis.Workspace;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    internal sealed class SmartIndent : ISmartIndent
    {
        private readonly WorkspaceDocument<ITextBuffer> document;
        private readonly int tabSize;

        public SmartIndent(WorkspaceDocument<ITextBuffer> document, int tabSize)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            this.tabSize = tabSize;
        }

        public void Dispose() => this.document.Dispose();

        public int? GetDesiredIndentation(ITextSnapshotLine line)
        {
            var docSnapshot = this.document.CurrentSnapshot;

            // TODO: ensure we're on the same snapshot as the document and track if needed.
            var lineStart = line.Start;

            if (docSnapshot.TryGetDeepestNode<BlockNode>(lineStart.Position, out var discoveredNode))
            {
                var endPoint = new SnapshotPoint(line.Snapshot, discoveredNode.Extent.End);
                var endLineStart = endPoint.GetContainingLine().Start;
                var endPointIndent = endPoint - endLineStart - 1;

                // TODO: for now use the end of the block's indent as the smart indent.
                // Eventually this should be the block's general indent plus 'one more level'.
                return endPointIndent + this.tabSize;
            }

            return null;
        }
    }
}
