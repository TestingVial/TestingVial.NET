namespace TestingVial.NET
{
    public class EndToEndTestVial<T> : TestingVialAttribute<T>
      where T : IVial, new()
    {

        public EndToEndTestVial() : base("E2E")
        {
        }

    }
    public class EndToEndTestVial : VialAttribute
    {
        public EndToEndTestVial(string name) : base(name, "E2E")
        {
        }
    }
}
