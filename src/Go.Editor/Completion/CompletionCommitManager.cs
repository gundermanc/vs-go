using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Go.Editor.Completion
{
    class CompletionCommitManager : IAsyncCompletionCommitManager
    {
        public CompletionCommitManager()
        {

        }

        public IEnumerable<char> PotentialCommitCharacters => new char [] { ' ' };

        public bool ShouldCommitCompletion(IAsyncCompletionSession session, SnapshotPoint location, char typedChar, CancellationToken token)
        {
            return true;
        }

        public CommitResult TryCommit(IAsyncCompletionSession session, ITextBuffer buffer, CompletionItem item, char typedChar, CancellationToken token)
        {
            return typedChar.Equals(' ') ? CommitResult.Handled : CommitResult.Unhandled;
        }
    }
}
