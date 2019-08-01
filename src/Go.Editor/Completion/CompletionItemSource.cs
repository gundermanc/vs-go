namespace Go.Editor.Completion
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Go.Interop.Workspace;
    using Microsoft.VisualStudio.Core.Imaging;
    using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
    using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Operations;

    internal sealed class CompletionSource : IAsyncCompletionSource
    {
        private static readonly ImageElement CompletionItemIcon
            = new ImageElement(new ImageId(new Guid("ae27a6b0-e345-4288-96df-5eaf394ee369"), 3335), "Hello Icon");
        private readonly ImmutableArray<CompletionItem> keywords;
        private readonly ITextStructureNavigatorSelectorService navigatorSelectorService;
        private readonly WorkspaceDocument<ITextBuffer> document;

        public CompletionSource(ITextStructureNavigatorSelectorService navigatorSelectorService, WorkspaceDocument<ITextBuffer> document)
        {
            this.navigatorSelectorService = navigatorSelectorService;
            this.document = document;

            this.keywords = ImmutableArray.Create(
                new CompletionItem("break", this, CompletionItemIcon),
                new CompletionItem("default", this, CompletionItemIcon),
                new CompletionItem("func", this, CompletionItemIcon),
                new CompletionItem("interface", this, CompletionItemIcon),
                new CompletionItem("select", this, CompletionItemIcon),
                new CompletionItem("case", this, CompletionItemIcon),
                new CompletionItem("defer", this, CompletionItemIcon),
                new CompletionItem("go", this, CompletionItemIcon),
                new CompletionItem("map", this, CompletionItemIcon),
                new CompletionItem("struct", this, CompletionItemIcon),
                new CompletionItem("chan", this, CompletionItemIcon),
                new CompletionItem("else", this, CompletionItemIcon),
                new CompletionItem("goto", this, CompletionItemIcon),
                new CompletionItem("package", this, CompletionItemIcon),
                new CompletionItem("switch", this, CompletionItemIcon),
                new CompletionItem("const", this, CompletionItemIcon),
                new CompletionItem("fallthrough", this, CompletionItemIcon),
                new CompletionItem("if", this, CompletionItemIcon),
                new CompletionItem("range", this, CompletionItemIcon),
                new CompletionItem("type", this, CompletionItemIcon),
                new CompletionItem("continue", this, CompletionItemIcon),
                new CompletionItem("for", this, CompletionItemIcon),
                new CompletionItem("import", this, CompletionItemIcon),
                new CompletionItem("return", this, CompletionItemIcon),
                new CompletionItem("var", this, CompletionItemIcon));
        }

        public CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation, CancellationToken token)
        {
            if (!char.IsLetterOrDigit(trigger.Character))
                return CompletionStartData.DoesNotParticipateInCompletion;

            var navigator = this.navigatorSelectorService.GetTextStructureNavigator(triggerLocation.Snapshot.TextBuffer);
            var extent = navigator.GetExtentOfWord(triggerLocation - 1);

            if (extent.IsSignificant)
            {
                var extentText = extent.Span.GetText();
                if (extentText.Length == 1 && !char.IsLetterOrDigit(extentText[0]))
                {
                    return new CompletionStartData(CompletionParticipation.ProvidesItems, new SnapshotSpan(triggerLocation, triggerLocation));
                }

                return new CompletionStartData(CompletionParticipation.ProvidesItems, extent.Span);
            }

            return new CompletionStartData(CompletionParticipation.ProvidesItems, new SnapshotSpan(triggerLocation, 0));
        }

        public async Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
        {
            var items = ImmutableArray.CreateBuilder<CompletionItem>();
            items.AddRange(keywords);
            foreach (var item in this.document.GetCompletions(triggerLocation.Position))
            {
                items.Add(new CompletionItem(item, this, CompletionItemIcon));
            }

            var result = new CompletionContext(items.ToImmutable());
            return await Task.FromResult(result);
        }

        public async Task<object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token)
        {
            var content = new ContainerElement(
                ContainerElementStyle.Wrapped,
                CompletionItemIcon,
                new ClassifiedTextElement(
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.Keyword, item.DisplayText)));

            var result = new ContainerElement(
                ContainerElementStyle.Stacked,
                content);

            return await Task.FromResult(result);
        }
    }
}
