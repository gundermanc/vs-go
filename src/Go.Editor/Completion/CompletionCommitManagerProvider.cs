namespace Go.Editor.Completion
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IAsyncCompletionCommitManagerProvider))]
    [ContentType(GoContentType.Name)]
    [Name("CommitManager")]
    internal sealed class CompletionCommitManagerProvider : IAsyncCompletionCommitManagerProvider
    {
        public IAsyncCompletionCommitManager GetOrCreate(ITextView textView)
            => textView.Properties.GetOrCreateSingletonProperty(
                typeof(CompletionCommitManager),
                () => new CompletionCommitManager());
    }
}
