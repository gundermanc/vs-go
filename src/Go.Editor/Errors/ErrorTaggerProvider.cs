namespace Go.Editor.Errors
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType(GoContentType.Name)]
    [TagType(typeof(IErrorTag))]
    internal sealed class ErrorTaggerProvider : ITaggerProvider
    {
        private readonly ITextStructureNavigatorSelectorService navigatorService;

        [ImportingConstructor]
        public ErrorTaggerProvider(ITextStructureNavigatorSelectorService navigatorService)
        {
            this.navigatorService = navigatorService;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer textBuffer) where T : ITag
        {
            return new ErrorTagger(
                textBuffer,
                this.navigatorService.CreateTextStructureNavigator(textBuffer, textBuffer.ContentType)) as ITagger<T>;
        }
    }
}
