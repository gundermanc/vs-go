namespace Go.Editor.SmartIndent
{
    using System;
    using Go.Editor.Common;
    using Go.Interop.Workspace;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    internal sealed class SmartIndent : ISmartIndent
    {
        private readonly WorkspaceDocument<ITextBuffer> document;
        private readonly IEditorOptions options;

        public SmartIndent(WorkspaceDocument<ITextBuffer> document, IEditorOptions options)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Dispose() => this.document.Dispose();

        public int? GetDesiredIndentation(ITextSnapshotLine line)
        {
            var snapshot = this.document.CurrentSnapshot.ToTextSnapshot();
            var closeBracePosition = this.document.GetPositionOfCloseBraceOfEnclosingBlock(line.Start.Position);
            if (closeBracePosition != -1)
            {
                var closeBraceSnapshotPoint = new SnapshotPoint(snapshot, closeBracePosition);
                var closeBraceIndent = closeBraceSnapshotPoint.Position - closeBraceSnapshotPoint.GetContainingLine().Start.Position;
                var indentSize = this.options.GetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId) ?
                    this.options.GetOptionValue(DefaultOptions.TabSizeOptionId) :
                    1;

                return closeBraceIndent + indentSize;
            }

            return null;
        }
    }
}
