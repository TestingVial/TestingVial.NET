using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TestingVial.NET.Analyzers
{
    /// <summary>
    /// Warns when a vial attribute has no <c>Description</c> (missing, null, empty or whitespace). The .vial format
    /// requires a description for every entry.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MissingDescriptionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "TV0001";

        private const string VialAttributeMetadataName = "TestingVial.NET.VialAttribute";
        private const string DescriptionProperty = "Description";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            title: "Vial attribute has no Description",
            messageFormat: "'{0}' has no Description; describe what the test verifies about the vial",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Every vial needs a description of what the test verifies. Without one, tools that read the "
                + "attributes (such as Pipette.NET) have to fall back to the name of the test method or class.",
            helpLinkUri: "https://github.com/TestingVial/TestingVial.NET#tv0001-vial-attribute-has-no-description");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(start =>
            {
                // Null when the compilation doesn't reference TestingVial.NET: nothing to analyze.
                var vialAttribute = start.Compilation.GetTypeByMetadataName(VialAttributeMetadataName);
                if (vialAttribute is not null)
                {
                    start.RegisterSyntaxNodeAction(c => Analyze(c, vialAttribute), SyntaxKind.Attribute);
                }
            });
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, INamedTypeSymbol vialAttribute)
        {
            var attribute = (AttributeSyntax)context.Node;
            if (context.SemanticModel.GetSymbolInfo(attribute, context.CancellationToken).Symbol is not IMethodSymbol constructor
                || !DerivesFrom(constructor.ContainingType, vialAttribute)
                || HasDescription(attribute, context.SemanticModel, context.CancellationToken))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, attribute.GetLocation(), attribute.Name.ToString()));
        }

        private static bool DerivesFrom(INamedTypeSymbol? type, INamedTypeSymbol baseType)
        {
            for (var current = type; current is not null; current = current.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasDescription(AttributeSyntax attribute, SemanticModel model, System.Threading.CancellationToken cancellationToken)
        {
            foreach (var argument in attribute.ArgumentList?.Arguments ?? default)
            {
                if (argument.NameEquals?.Name.Identifier.ValueText != DescriptionProperty)
                {
                    continue;
                }

                // Attribute arguments are constants; anything else is a compile error reported elsewhere.
                var value = model.GetConstantValue(argument.Expression, cancellationToken);
                return !value.HasValue || !string.IsNullOrWhiteSpace(value.Value as string);
            }

            return false;
        }
    }
}
