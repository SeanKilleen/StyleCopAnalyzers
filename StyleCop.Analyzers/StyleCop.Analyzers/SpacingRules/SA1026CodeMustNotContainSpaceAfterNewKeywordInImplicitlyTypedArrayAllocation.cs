﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An implicitly typed new array allocation within a C# code file is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains an implicitly typed new array allocation which
    /// is not spaced correctly. Within an implicitly typed new array allocation, there should not be any space between
    /// the new keyword and the opening array bracket. For example:</para>
    ///
    /// <code language="cs">
    /// var a = new[] { 1, 10, 100, 1000 };
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1026";
        internal const string Title = "Code Must Not Contain Space After New Keyword In Implicitly Typed Array Allocation";
        internal const string MessageFormat = "The keyword 'new' must not be followed by a space.";
        internal const string Category = "StyleCop.CSharp.Spacing";
        internal const string Description = "An implicitly typed new array allocation within a C# code file is not spaced correctly.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1026.html";

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
            foreach (var token in root.DescendantTokens())
            {
                switch (token.CSharpKind())
                {
                case SyntaxKind.NewKeyword:
                    HandleNewKeywordToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleNewKeywordToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            if (!token.Parent.IsKind(SyntaxKind.ImplicitArrayCreationExpression))
                return;

            HandleDisallowedSpaceToken(context, token);
        }

        private void HandleDisallowedSpaceToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing || !token.HasTrailingTrivia)
                return;

            if (!token.TrailingTrivia.First().IsKind(SyntaxKind.WhitespaceTrivia))
                return;

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));
        }
    }
}
