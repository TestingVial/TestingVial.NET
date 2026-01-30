namespace TestingVial.NET
{
    public class UnitTestVial<T> : TestingVialAttribute<T>
     where T : IVial, new()
    {

        public UnitTestVial() : base("Unit")
        {
        }

    }
    public class UnitTestVial : VialAttribute
    {
        public UnitTestVial(string name) : base(name, "Unit")
        {
        }
    }
}
