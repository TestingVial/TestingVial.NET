namespace TestingVial.NET
{
    public abstract class TestingVialAttribute<T> : VialAttribute
       where T : IVial, new()
    {
        public TestingVialAttribute(string testType) : base((new T()).Name, testType)
        {
        }
    }
}
