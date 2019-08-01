namespace Go.Interop.Workspace
{
    using System;
    using Go.Interop.Text;

    public sealed class WorkspaceDocumentUpdatedEventArgs<TKey> : EventArgs
    {
        public WorkspaceDocumentUpdatedEventArgs(string filePath, SnapshotBase snapshot)
        {
            this.FilePath = filePath
                ?? throw new ArgumentNullException(nameof(filePath));
            this.Snapshot = snapshot
                ?? throw new ArgumentNullException(nameof(snapshot));
        }

        public string FilePath { get; }

        public SnapshotBase Snapshot { get; }
    }
}
