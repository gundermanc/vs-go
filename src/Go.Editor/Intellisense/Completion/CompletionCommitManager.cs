namespace Go.Editor.Intellisense.Completion
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
    using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
    using Microsoft.VisualStudio.Text;

    internal sealed class CompletionCommitManager : IAsyncCompletionCommitManager
    {
        public CompletionCommitManager()
        {
        }

        public IEnumerable<char> PotentialCommitCharacters => new char [] { ' ' };

        public bool ShouldCommitCompletion(IAsyncCompletionSession session, SnapshotPoint location, char typedChar, CancellationToken token) => true;

        public CommitResult TryCommit(IAsyncCompletionSession session, ITextBuffer buffer, CompletionItem item, char typedChar, CancellationToken token)
        {
            return typedChar.Equals(' ') ? CommitResult.Handled : CommitResult.Unhandled;
        }
    }
}
