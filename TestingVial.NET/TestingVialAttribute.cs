namespace TestingVial.NET
{
    /// <summary>
    /// A generic vial with no test type, e.g. <c>[TestingVial&lt;CartVial&gt;]</c>; also the base class for typed vials.
    /// </summary>
    public class TestingVialAttribute<T> : VialAttribute
       where T : IVial, new()
    {
        public TestingVialAttribute() : base((new T()).Name)
        {
        }

        protected TestingVialAttribute(string testType) : base((new T()).Name, testType)
        {
        }
    }

    /// <summary>A generic vial with no test type, e.g. <c>[TestingVial("Cart")]</c>.</summary>
    public class TestingVialAttribute : VialAttribute
    {
        public TestingVialAttribute(string name) : base(name)
        {
        }
    }
}
