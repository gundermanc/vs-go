namespace Go.CodeAnalysis.Workspace
{
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Text;

    public abstract class WorkspaceBase<TKey>
    {
        private ImmutableDictionary<TKey, WorkspaceDocument<TKey>> documents
            = ImmutableDictionary<TKey, WorkspaceDocument<TKey>>.Empty;

        public ImmutableDictionary<TKey, WorkspaceDocument<TKey>> Documents => this.documents;

        public abstract SnapshotBase GetCurrentSnapshot(WorkspaceDocument<TKey> workspaceDocumentBase);

        public WorkspaceDocument<TKey> GetOrCreateDocument(TKey key)
        {
            var document = ImmutableInterlocked.GetOrAdd(ref this.documents, key, new WorkspaceDocument<TKey>(this, key));

            document.AddRef();
            document.QueueReparse();

            this.AttachDocument(document);

            return document;
        }

        internal bool TryRemoveDocument(TKey key)
        {
            return ImmutableInterlocked.TryRemove(ref this.documents, key, out _);
        }

        protected abstract void AttachDocument(WorkspaceDocument<TKey> document);
    }
}
