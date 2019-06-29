namespace Go.Editor.TextMate
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// TextMate is a Visual Studio Core Editor feature that provides rudimentary colorization,
    /// outlining, completion, tag span highlighting, etc. based on text-based heuristics like
    /// indentation. As we build up a parser, we should replace these bits with features backed
    /// by the language service, but this will work for now. Currently we're using:
    ///
    /// - Indent based outlining and structure guides.
    ///
    /// NOTE: this feature may not be supported by VS Mac, so this may potentially be an adoption
    /// blocker.
    /// </summary>
    [Export(typeof(ITaggerProvider))]
    [ContentType(GoContentType.Name)]
    [TagType(typeof(IStructureTag))]
    internal sealed class TextMateStubsTagger : ITaggerProvider
    {
        private readonly ICommonEditorAssetServiceFactory assetServiceFactory;

        [ImportingConstructor]
        public TextMateStubsTagger(ICommonEditorAssetServiceFactory assetServiceFactory)
        {
            this.assetServiceFactory = assetServiceFactory
                ?? throw new ArgumentNullException(nameof(assetServiceFactory));
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag =>
            this.assetServiceFactory.GetOrCreate(buffer)
            .FindAsset<ITaggerProvider>(
                (metadata) => metadata.TagTypes.Any(tagType => typeof(T).IsAssignableFrom(tagType)))
            ?.CreateTagger<T>(buffer);
    }
}
