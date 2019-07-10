namespace Go.Editor.Errors
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// Hacky stand-in for real error handling that uses Gofmt.exe for basic syntax errors.
    /// TODO: delete this once our parser is mature enough to give similar errors. For now we have both.
    /// </summary>

    // TODO: enable for Mac or remove.
    //[Export(typeof(ITaggerProvider))]
    [ContentType(GoContentType.Name)]
    [TagType(typeof(IErrorTag))]
    internal sealed class GoFmtErrorTaggerProvider : ITaggerProvider
    {
        private readonly ITextStructureNavigatorSelectorService navigatorService;

        [ImportingConstructor]
        public GoFmtErrorTaggerProvider(ITextStructureNavigatorSelectorService navigatorService)
        {
            this.navigatorService = navigatorService;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer textBuffer) where T : ITag
        {
            return new GoFmtErrorTagger(
                textBuffer,
                this.navigatorService.CreateTextStructureNavigator(textBuffer, textBuffer.ContentType)) as ITagger<T>;
        }
    }
}
