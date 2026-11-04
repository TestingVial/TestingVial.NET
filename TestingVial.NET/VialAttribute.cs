namespace TestingVial.NET
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public abstract class VialAttribute : Attribute
    {
        /// <summary>A generic vial: it belongs to <paramref name="name"/> but has no test type.</summary>
        protected VialAttribute(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            VialName = name;
        }

        protected VialAttribute(string name, string testType)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(testType);
            VialName = name;
            TestType = testType;
        }

        public string VialName { get; }

        /// <summary>The test type, e.g. <c>"Unit"</c>; <see langword="null"/> for generic vials.</summary>
        public string? TestType { get; }
        public string? Description { get; init; }
    }



}
