namespace Go.Interop.Workspace
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using Go.Interop.Text;

    public sealed class WorkspaceDocument<TKey> : IDisposable
    {
        private readonly WorkspaceBase<TKey> workspace;
        private readonly byte[] utf8Key;
        private int refCount;

        public event EventHandler DocumentUpdated;

        public TKey Key { get; }

        public SnapshotBase CurrentSnapshot { get; private set; }

        public WorkspaceDocument(WorkspaceBase<TKey> workspace, TKey key)
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            this.Key = key;
            this.utf8Key = Encoding.UTF8.GetBytes(this.workspace.KeyToString(key));
            this.CurrentSnapshot = workspace.GetCurrentSnapshot(this);
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

            var currentSnapshot = this.workspace.GetCurrentSnapshot(this);

            NativeMethods.QueueFileParse(this.workspace.WorkspaceId, utf8Key, utf8Key.Length, InteropSnapshot.FromSnapshot(currentSnapshot), currentSnapshot);
        }

        public unsafe IList<string> GetCompletions(int position)
        {
            var completions = new List<string>();

            void CompletionsCallback(byte* errorText, int length)
            {
                completions.Add(Encoding.UTF8.GetString(errorText, length));
            }

            NativeMethods.GetCompletions(this.workspace.WorkspaceId, utf8Key, utf8Key.Length, CompletionsCallback, position);
            return completions;
        }

        public IList<TypedToken> GetFileTokens()
        {
            var tokens = new List<TypedToken>();

            void TokensCallback(int pos, int end, int type)
            {
                tokens.Add(new TypedToken(pos, end, (TokenType)type));
            }

            NativeMethods.GetTokens(this.workspace.WorkspaceId, this.utf8Key, this.utf8Key.Length, TokensCallback);
            return tokens;
        }

        public unsafe IList<string> GetErrors()
        {
            var errors = new List<string>();

            void ErrorCallback(byte* errorText, int length)
            {
                errors.Add(Encoding.UTF8.GetString(errorText, length));
            }

            NativeMethods.GetErrors(this.workspace.WorkspaceId, this.utf8Key, this.utf8Key.Length, ErrorCallback);
            return errors;
        }

        internal void RaiseDocumentUpdated(SnapshotBase snapshot)
        {
            this.CurrentSnapshot = snapshot;
            this.DocumentUpdated?.Invoke(this, EventArgs.Empty);
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
