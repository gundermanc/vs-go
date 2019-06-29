namespace Go.Editor.Intellisense.QuickInfo
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [ContentType(GoContentType.Name)]
    internal sealed class AsyncQuickInfoItemsSourceProvider : IAsyncQuickInfoSourceProvider
    {
        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) => new AsyncQuickInfoSource(textBuffer);
    }
}
