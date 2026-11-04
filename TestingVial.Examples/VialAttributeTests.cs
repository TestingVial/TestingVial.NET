using TestingVial.NET;

namespace TestingVial.Examples
{
    /// <summary>
    /// The test type strings end up in .vial files (Pipette.NET lowercases them), so they are part of the public contract.
    /// </summary>
    public class VialAttributeTests
    {
        private static IEnumerable<TestCaseData> BuiltInVials()
        {
            yield return new TestCaseData(new UnitTestVial<CartVial>(), "Cart", "Unit");
            yield return new TestCaseData(new UnitTestVial("Product"), "Product", "Unit");
            yield return new TestCaseData(new IntegrationTestVial<CartVial>(), "Cart", "Integration");
            yield return new TestCaseData(new IntegrationTestVial("Product"), "Product", "Integration");
            yield return new TestCaseData(new EndToEndTestVial<CartVial>(), "Cart", "E2E");
            yield return new TestCaseData(new EndToEndTestVial("Product"), "Product", "E2E");
        }

        [TestCaseSource(nameof(BuiltInVials))]
        public void BuiltInVial_HasVialNameAndTestType(VialAttribute vial, string expectedName, string expectedTestType)
        {
            Assert.That(vial.VialName, Is.EqualTo(expectedName));
            Assert.That(vial.TestType, Is.EqualTo(expectedTestType));
        }
    }
}
