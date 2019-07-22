using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace Go.Editor.Completion
{
    [Export(typeof(IAsyncCompletionSourceProvider))]
    [ContentType(GoContentType.Name)]
    [Name("Go Completion")]
    internal sealed class CompletionSourceProvider : IAsyncCompletionSourceProvider
    {
        private readonly Dictionary<ITextView, CompletionSource> sourceMap = new Dictionary<ITextView, CompletionSource>();

        [Import]
        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService { get; set; }

        public IAsyncCompletionSource GetOrCreate(ITextView textView)
        {
            CompletionSource source;
            if (!this.sourceMap.TryGetValue(textView, out source))
            {
                source = new CompletionSource(this.TextStructureNavigatorSelectorService);
                sourceMap.Add(textView, source);

                textView.Options.SetOptionValue(DefaultOptions.NonBlockingCompletionOptionId, true);
                textView.Closed += OnTextViewClosed;
            }

            return source;
        }

        private void OnTextViewClosed(object sender, System.EventArgs e)
        {
            try
            {
                if (sender is ITextView textView)
                {
                    textView.Options.SetOptionValue(DefaultOptions.NonBlockingCompletionOptionId, false);
                    this.sourceMap.Remove(textView);
                }
            }
            catch
            {

            }
        }
    }
}
