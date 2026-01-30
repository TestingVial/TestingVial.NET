namespace TestingVial.NET
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public abstract class VialAttribute : Attribute
    {
        protected VialAttribute(string name, string testType)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(testType);
            VialName = name;
            TestType = testType;
        }

        public string VialName { get; }
        public string TestType { get; }
        public string? Description { get; init; }
    }



}
