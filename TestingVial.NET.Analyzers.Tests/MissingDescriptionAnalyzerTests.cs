using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TestingVial.NET.Analyzers.Tests
{
    public class MissingDescriptionAnalyzerTests
    {
        private const string Header = """
            using TestingVial.NET;
            public class CartVial : IVial { public string Name => "Cart"; }
            public static class Texts { public const string Empty = ""; public const string Added = "Products can be added"; }
            public class PerformanceTestVial : VialAttribute { public PerformanceTestVial(string name) : base(name, "Performance") { } }
            public class SmokeVial<T> : TestingVialAttribute<T> where T : IVial, new() { public SmokeVial() : base("Smoke") { } }

            """;

        // 1-based line of the first line after the header.
        private static readonly int FirstLine = Header.Split('\n').Length;

        [TestCase("UnitTestVial<CartVial>")]
        [TestCase("UnitTestVial<CartVial>()")]
        [TestCase("UnitTestVial(\"Cart\")")]
        [TestCase("IntegrationTestVial<CartVial>(Description = \"\")")]
        [TestCase("IntegrationTestVial(\"Cart\", Description = \"   \")")]
        [TestCase("EndToEndTestVial<CartVial>(Description = null)")]
        [TestCase("EndToEndTestVial(\"Cart\", Description = Texts.Empty)")]
        [TestCase("TestingVial.NET.UnitTestVial(\"Cart\")")]
        [TestCase("PerformanceTestVial(\"Cart\")")]
        [TestCase("SmokeVial<CartVial>")]
        [TestCase("TestingVial<CartVial>")]
        [TestCase("TestingVial(\"Cart\")")]
        public async Task VialAttribute_WithoutDescription_Warns(string attribute)
        {
            var source = Header + $"public class CartTests {{ [{attribute}] public void M() {{ }} }}";

            var diagnostic = (await AnalyzeAsync(source)).Single();

            Assert.Multiple(() =>
            {
                Assert.That(diagnostic.Id, Is.EqualTo(MissingDescriptionAnalyzer.DiagnosticId));
                Assert.That(diagnostic.Severity, Is.EqualTo(DiagnosticSeverity.Warning));
                Assert.That(source.Substring(diagnostic.Location.SourceSpan.Start, diagnostic.Location.SourceSpan.Length), Is.EqualTo(attribute));
                Assert.That(diagnostic.GetMessage(), Does.Contain("has no Description"));
            });
        }

        [Test]
        public async Task EveryAttributeWithoutDescription_OnClassesAndMethods_IsReported()
        {
            var diagnostics = await AnalyzeAsync(Header + """
                [UnitTestVial<CartVial>]
                public class CartTests
                {
                    [UnitTestVial("Cart"), IntegrationTestVial("Cart", Description = "d")]
                    [EndToEndTestVial<CartVial>]
                    public void M() { }
                }
                """);

            Assert.That(
                diagnostics.Select(d => d.Location.GetLineSpan().StartLinePosition.Line + 1).Order(),
                Is.EqualTo(new[] { FirstLine, FirstLine + 3, FirstLine + 4 }));
        }

        [TestCase("UnitTestVial<CartVial>(Description = \"Carts can have products added\")")]
        [TestCase("UnitTestVial(\"Cart\", Description = Texts.Added)")]
        [TestCase("PerformanceTestVial(\"Cart\", Description = \"Fast\")")]
        [TestCase("SmokeVial<CartVial>(Description = \"Smoke\")")]
        [TestCase("TestingVial<CartVial>(Description = \"Carts can have products added\")")]
        [TestCase("TestingVialAttribute(\"Cart\", Description = \"Carts can have products added\")")]
        [TestCase("System.Obsolete")]
        public async Task DescribedVialsAndOtherAttributes_DoNotWarn(string attribute)
        {
            var diagnostics = await AnalyzeAsync(Header + $"public class CartTests {{ [{attribute}] public void M() {{ }} }}");

            Assert.That(diagnostics, Is.Empty);
        }

        [Test]
        public async Task CompilationWithoutTestingVial_DoesNotWarn()
        {
            var diagnostics = await AnalyzeAsync(
                "public class UnitTestVial : System.Attribute { } [UnitTestVial] public class T { }", referenceTestingVial: false);

            Assert.That(diagnostics, Is.Empty);
        }

        private static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(string source, bool referenceTestingVial = true)
        {
            var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
                .Split(Path.PathSeparator)
                .Where(path => !Path.GetFileName(path).StartsWith("TestingVial.NET", StringComparison.Ordinal))
                .Select(path => MetadataReference.CreateFromFile(path))
                .ToList();
            if (referenceTestingVial)
            {
                references.Add(MetadataReference.CreateFromFile(typeof(VialAttribute).Assembly.Location));
            }

            var compilation = CSharpCompilation.Create(
                "AnalyzerTest",
                [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest))],
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

            var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            Assert.That(errors, Is.Empty, string.Join(Environment.NewLine, errors));

            return await compilation
                .WithAnalyzers([new MissingDescriptionAnalyzer()])
                .GetAnalyzerDiagnosticsAsync();
        }
    }
}
