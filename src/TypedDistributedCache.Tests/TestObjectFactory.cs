namespace TypedDistributedCache.Tests
{
    public class TestObjectFactory
    {
        public static TestObject Create()
        {
            var testObject = new TestObject
            {
                Name = "Jarrich"
            };

            return testObject;
        }
    }

    public class TestObject
    {
        public string Name { get; set; }
    }
}
