using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Go.Editor.Completion
{
    [Export(typeof(IAsyncCompletionSourceProvider))]
    [ContentType(GoContentType.Name)]
    [Name("Go Completion")]
    internal sealed class CompletionSourceProvider : IAsyncCompletionSourceProvider
    {
        //Lazy<CompletionSource> Source = new Lazy<CompletionSource>(() => new CompletionSource());

        [Import]
        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService { get; set; }

        /*public IAsyncCompletionSource GetOrCreate(ITextView textView)
        {
            return Source.Value;
        }*/


        private readonly Dictionary<ITextView, CompletionSource> sourceMap = new Dictionary<ITextView, CompletionSource>();

        public IAsyncCompletionSource GetOrCreate(ITextView textView)
        {
            CompletionSource source;
            if (!this.sourceMap.TryGetValue(textView, out source))
            {
                source = new CompletionSource(this.TextStructureNavigatorSelectorService);
                sourceMap.Add(textView, source);

                // We want to make sure we don't block on commit as language servers can take a while to return results.
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
                // Any exceptions caught here should just be swallowed. We may be in a partially disposed state with the editor, and ObjectDisposedException may be thrown.
            }
        }
    }
}
