namespace Go.CodeAnalysis.Workspace
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Go.CodeAnalysis.Parser;

    public sealed class WorkspaceDocument<TKey> : IDisposable
    {
        private readonly WorkspaceBase<TKey> workspace;
        private ParseSnapshot currentSnapshot;
        private int queuedReparses = 0;
        private int refCount;

        public event EventHandler SnapshotUpdated;

        public TKey Key { get; }

        public ParseSnapshot CurrentSnapshot
        {
            get
            {
                this.ThrowIfDisposed();
                return this.currentSnapshot;
            }
        }

        public WorkspaceDocument(WorkspaceBase<TKey> workspace, TKey key)
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            this.Key = key;
        }

        internal void AddRef() => Interlocked.Increment(ref this.refCount);

        public void Dispose()
        {
            this.ThrowIfDisposed();

            if (Interlocked.Decrement(ref this.refCount) == 0)
            {
                while (!this.workspace.TryRemoveDocument(this.Key));
            }
        }

        public void QueueReparse()
        {
            this.ThrowIfDisposed();

            if (Interlocked.Increment(ref this.queuedReparses) == 1)
            {
                Task.Run(() => this.Reparse());
            }
        }

        private void Reparse()
        {
            // TODO: delay until user stops typing?

            while (Interlocked.Exchange(ref this.queuedReparses, 0) > 0)
            {
                // TODO: probably should cancel if the document is closed.
                var currentSnapsnot = this.workspace.GetCurrentSnapshot(this);

                this.currentSnapshot = ParseSnapshot.Create(currentSnapsnot);
                this.SnapshotUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.refCount == 0)
            {
                throw new ObjectDisposedException(nameof(WorkspaceDocument<TKey>));
            }
        }
    }
}
