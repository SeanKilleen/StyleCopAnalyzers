﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The C# code contains a tab character.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a tab character.</para>
    ///
    /// <para>Tabs should not be used within C# code, because the length of the tab character can vary depending upon
    /// the editor being used to view the code. This can cause the spacing and indexing of the code to vary from the
    /// developer's original intention, and can in some cases make the code difficult to read.</para>
    ///
    /// <para>For these reasons, tabs should not be used, and each level of indentation should consist of four spaces.
    /// This will ensure that the code looks the same no matter which editor is being used to view the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1027TabsMustNotBeUsed : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1027";
        internal const string Title = "Tabs Must Not Be Used";
        internal const string MessageFormat = "Tabs must not be used.";
        internal const string Category = "StyleCop.CSharp.Spacing";
        internal const string Description = "The C# code contains a tab character.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1027.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
            {
                switch (trivia.CSharpKind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    HandleWhitespaceTrivia(context, trivia);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleWhitespaceTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia trivia)
        {
            if (trivia.ToFullString().IndexOf('\t') < 0)
                return;

            // Tabs must not be used.
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, trivia.GetLocation()));
        }
    }
}
