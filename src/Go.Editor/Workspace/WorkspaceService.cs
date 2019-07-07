namespace Go.Editor.Workspace
{
    using System.ComponentModel.Composition;
    using Go.CodeAnalysis.Text;
    using Go.CodeAnalysis.Workspace;
    using Go.Editor.Common;
    using Microsoft.VisualStudio.Text;

    [Export(typeof(WorkspaceService))]
    internal sealed class WorkspaceService : WorkspaceBase<ITextBuffer>
    {
        public override SnapshotBase GetCurrentSnapshot(WorkspaceDocument<ITextBuffer> workspaceDocumentBase)
            => workspaceDocumentBase.Key.CurrentSnapshot.ToSnapshot();

        protected override void AttachDocument(WorkspaceDocument<ITextBuffer> document)
        {
            // TODO: unsubscribe?
            document.Key.Changed += (sender, args) => document.QueueReparse();
        }
    }
}
