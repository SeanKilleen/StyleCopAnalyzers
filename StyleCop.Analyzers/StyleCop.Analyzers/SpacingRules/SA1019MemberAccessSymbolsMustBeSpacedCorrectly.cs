﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The spacing around a member access symbol is incorrect, within a C# code file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a member access symbol is incorrect. A member
    /// access symbol should not have whitespace on either side, unless it is the first character on the line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1019MemberAccessSymbolsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1019";
        internal const string Title = "Member Access Symbols Must Be Spaced Correctly";
        internal const string MessageFormat = "Member access symbol '{0}' must not be {1} by a space.";
        internal const string Category = "StyleCop.CSharp.Spacing";
        internal const string Description = "The spacing around a member access symbol is incorrect, within a C# code file.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1019.html";

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
                case SyntaxKind.DotToken:
                    HandleDotToken(context, token);
                    break;

                // This case handles the new ?. and ?[ operators
                case SyntaxKind.QuestionToken:
                    HandleQuestionToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private void HandleDotToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            HandleMemberAccessSymbol(context, token);
        }

        private void HandleQuestionToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
                return;

            if (!token.Parent.IsKind(SyntaxKind.ConditionalAccessExpression))
                return;

            HandleMemberAccessSymbol(context, token);
        }

        private void HandleMemberAccessSymbol(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            bool precededBySpace;
            bool firstInLine;

            bool followedBySpace;

            firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
            if (firstInLine)
            {
                precededBySpace = true;
            }
            else
            {
                SyntaxToken precedingToken = token.GetPreviousToken();
                precededBySpace = precedingToken.HasTrailingTrivia;
            }

            followedBySpace = token.HasTrailingTrivia;

            if (!firstInLine && precededBySpace)
            {
                // Member access symbol '{.}' must not be {preceded} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), token.Text, "preceded"));
            }

            if (followedBySpace)
            {
                // Member access symbol '{.}' must not be {followed} by a space.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), token.Text, "followed"));
            }
        }
    }
}
