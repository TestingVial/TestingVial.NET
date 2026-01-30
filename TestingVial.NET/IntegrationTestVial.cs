namespace TestingVial.NET
{
    public class IntegrationTestVial<T> : TestingVialAttribute<T>
      where T : IVial, new()
    {

        public IntegrationTestVial() : base("Integration")
        {
        }

    }
    public class IntegrationTestVial : VialAttribute
    {
        public IntegrationTestVial(string name) : base(name, "Integration")
        {
        }
    }
}
