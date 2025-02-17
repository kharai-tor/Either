using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace RhymesOfUncertainty;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RedundantCaseCodeFixProvider)), Shared]
public class RedundantCaseCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [EitherAnalyzer.RedundantCaseId];

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var label = root.FindToken(diagnosticSpan.Start).Parent.FirstAncestorOrSelf<CasePatternSwitchLabelSyntax>();

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Remove redundant case",
                createChangedDocument: c => RemoveCase(context.Document, label, c)),
        diagnostic);
    }

    private async Task<Document> RemoveCase(Document document, CasePatternSwitchLabelSyntax label, CancellationToken cancellationToken)
    {
        var switchSection = (SwitchSectionSyntax)label.Parent;

        var root = await document.GetSyntaxRootAsync().ConfigureAwait(false);
        var newRoot = switchSection.Labels.Count == 1 ?
            root.RemoveNode(switchSection, SyntaxRemoveOptions.KeepNoTrivia) :
            root.RemoveNode(label, SyntaxRemoveOptions.KeepNoTrivia);

        return document.WithSyntaxRoot(newRoot);
    }
}