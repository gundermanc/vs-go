namespace Go.Editor
{
    using System;
    using System.ComponentModel.Composition;
    using Go.Editor.Common;
    using Go.Interop.Text;
    using Go.Interop.Workspace;
    using Microsoft.VisualStudio.Text;

    [Export(typeof(GoWorkspace))]
    internal sealed class GoWorkspace : WorkspaceBase<ITextBuffer>
    {
        private readonly ITextDocumentFactoryService documentFactoryService;

        [ImportingConstructor]
        public GoWorkspace(ITextDocumentFactoryService documentFactoryService) : base()
        {
            this.documentFactoryService = documentFactoryService
                ?? throw new ArgumentNullException(nameof(documentFactoryService));
        }

        public override SnapshotBase GetCurrentSnapshot(WorkspaceDocument<ITextBuffer> workspaceDocumentBase)
            => workspaceDocumentBase.Key.CurrentSnapshot.ToSnapshot();

        public override string KeyToString(ITextBuffer key)
        {
            if (this.documentFactoryService.TryGetTextDocument(key, out var document))
            {
                return document.FilePath;
            }
            else
            {
                return key.Properties.GetOrCreateSingletonProperty("GoBufferId", () => Guid.NewGuid().ToString());
            }
        }

        protected override void AttachDocument(WorkspaceDocument<ITextBuffer> document)
        {
            base.AttachDocument(document);

            // TODO: is this a leak?
            document.Key.Changed += (sender, e) =>
            {
                document.QueueReparse();
            };
        }
    }
}
