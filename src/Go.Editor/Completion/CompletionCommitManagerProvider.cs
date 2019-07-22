using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Go.Editor.Completion
{
    [Export(typeof(IAsyncCompletionCommitManagerProvider))]
    [ContentType(GoContentType.Name)]
    [Name("CommitManager")]
    internal class CompletionCommitManagerProvider : IAsyncCompletionCommitManagerProvider
    {
        private readonly Dictionary<ITextView, CompletionCommitManager> managerMap = new Dictionary<ITextView, CompletionCommitManager>();

        public IAsyncCompletionCommitManager GetOrCreate(ITextView textView)
        {
            CompletionCommitManager manager;
            if (!this.managerMap.TryGetValue(textView, out manager))
            {
                manager = new CompletionCommitManager();
                managerMap.Add(textView, manager);
                textView.Closed += OnTextViewClosed;
            }

            return manager;
        }

        private void OnTextViewClosed(object sender, System.EventArgs e)
        {
            if (sender is ITextView textView)
            {
                this.managerMap.Remove(textView);
            }
        }
    }
}
