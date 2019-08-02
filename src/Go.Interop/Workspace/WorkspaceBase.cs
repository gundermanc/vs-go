namespace Go.Interop.Workspace
{
    using System;
    using System.Collections.Immutable;
    using System.Runtime.InteropServices;
    using System.Text;
    using Go.Interop.Text;

    public abstract class WorkspaceBase<TKey>
    {
        // Ensure callback is not GC-ed after passing to GO.
        private readonly NativeMethods.WorkspaceUpdatedCallback workspaceUpdatedDelegate;

        private ImmutableDictionary<TKey, WorkspaceDocument<TKey>> documents
            = ImmutableDictionary<TKey, WorkspaceDocument<TKey>>.Empty;

        public event EventHandler<WorkspaceDocumentUpdatedEventArgs<TKey>> WorkspaceFileUpdated;

        public int WorkspaceId { get; }

        protected unsafe WorkspaceBase()
        {
            this.WorkspaceId = NativeMethods.CreateNewWorkspace();
            this.workspaceUpdatedDelegate = this.WorkspaceUpdated;
            NativeMethods.RegisterWorkspaceUpdateCallback(this.WorkspaceId, this.workspaceUpdatedDelegate);
        }

        public WorkspaceDocument<TKey> GetOrCreateDocument(TKey key)
        {
            var document = ImmutableInterlocked.GetOrAdd(ref this.documents, key, new WorkspaceDocument<TKey>(this, key));

            document.AddRef();
            document.QueueReparse();
            this.AttachDocument(document);

            return document;
        }

        protected virtual void AttachDocument(WorkspaceDocument<TKey> document)
        {
            // TODO: unsubscribe?
            this.WorkspaceFileUpdated += (sender, e) =>
            {
                // Update features tracking particular files.
                if (e.FilePath == this.KeyToString(document.Key))
                {
                    document.RaiseDocumentUpdated(e.Snapshot);
                }
            };
        }

        public abstract SnapshotBase GetCurrentSnapshot(WorkspaceDocument<TKey> workspaceDocumentBase);

        public abstract string KeyToString(TKey key);

        internal bool TryRemoveDocument(TKey key) => ImmutableInterlocked.TryRemove(ref this.documents, key, out _);

        private unsafe void WorkspaceUpdated(byte* fileName, int length, IntPtr snapshot)
        {
            var fileNameString = Encoding.UTF8.GetString(fileName, length);

            // TODO: free.
            var handle = GCHandle.FromIntPtr(snapshot);
            this.WorkspaceFileUpdated?.Invoke(this, new WorkspaceDocumentUpdatedEventArgs<TKey>(fileNameString, (SnapshotBase)handle.Target));
        }
    }
}
