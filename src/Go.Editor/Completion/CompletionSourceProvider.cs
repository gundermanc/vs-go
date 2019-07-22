using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace Go.Editor.Completion
{
    [Export(typeof(IAsyncCompletionSourceProvider))]
    [ContentType("CSharp")]
    [Name("Completion item source")]
    public class CompletionSourceProvider : IAsyncCompletionSourceProvider
    {
        Lazy<CompletionSource> Source = new Lazy<CompletionSource>(() => new CompletionSource());

        public IAsyncCompletionSource GetOrCreate(ITextView textView)
        {
            return Source.Value;
        }
    }
}
