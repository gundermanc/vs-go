using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;

namespace Go.Editor.Completion
{
    public class CompletionSource : IAsyncCompletionSource
    {
        private static ImageElement CompletionItemIcon = new ImageElement(new ImageId(new Guid("ae27a6b0-e345-4288-96df-5eaf394ee369"), 3335), "Hello Icon");
        private ImmutableArray<CompletionItem> keywords;

        public CompletionSource()
        {
            keywords = ImmutableArray.Create(
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
            // Since we are plugging in to CSharp content type,
            // allow the CSharp language service to pick the Applicable To Span.
            return new CompletionStartData(CompletionParticipation.ProvidesItems, new SnapshotSpan(triggerLocation, 1));
            // Alternatively, we've got to provide location for completion
            // return new CompletionStartData(CompletionParticipation.ProvidesItems, ...
        }

        public async Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
        {
            session.Properties["LineNumber"] = triggerLocation.GetContainingLine().LineNumber;
            keywords.Where(keyword => keyword.DisplayText.Contains(applicableToSpan.GetText()));

            var result = new CompletionContext(keywords);
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
