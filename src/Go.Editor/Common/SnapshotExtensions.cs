namespace Go.Editor.Common
{
    using System;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;

    internal static class SnapshotExtensions
    {
        public static SnapshotBase ToSnapshot(this ITextSnapshot snapshot) => new TextSnapshot(snapshot);

        public static ITextSnapshot ToTextSnapshot(this SnapshotBase snapshot)
        {
            if (snapshot is TextSnapshot textSnapshot)
            {
                return textSnapshot.Snapshot;
            }

            throw new ArgumentException($"{nameof(snapshot)} must be of type {nameof(TextSnapshot)}");
        }

        public static SnapshotSegment ToSegment(this SnapshotSpan snapshotSpan)
        {
            return new SnapshotSegment(snapshotSpan.Snapshot.ToSnapshot(), snapshotSpan.Start, snapshotSpan.Length);
        }

        public static SnapshotSpan ToSnapshotSpan(this SnapshotSegment segment)
        {
            return new SnapshotSpan(segment.Snapshot.ToTextSnapshot(), segment.Start, segment.Length);
        }

        public static string ToClassificationTypeName(this LexemeType type)
        {
            switch (type)
            {
                case LexemeType.LineComment:
                    return PredefinedClassificationTypeNames.Comment;
                case LexemeType.GeneralComment:
                    return PredefinedClassificationTypeNames.Comment;
                case LexemeType.Identifier:
                    return PredefinedClassificationTypeNames.Identifier;
                case LexemeType.Keyword:
                    return PredefinedClassificationTypeNames.Keyword;
                case LexemeType.String:
                    return PredefinedClassificationTypeNames.String;
                case LexemeType.Operator:
                    return PredefinedClassificationTypeNames.Operator;
                case LexemeType.Integer:
                    return PredefinedClassificationTypeNames.Literal;
            }

            return PredefinedClassificationTypeNames.Other;
        }
    }
}
