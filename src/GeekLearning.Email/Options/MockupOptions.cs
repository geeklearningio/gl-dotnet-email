namespace GeekLearning.Email
{
    using System.Collections.Generic;

    public class MockupOptions
    {
        public List<string> Recipients { get; set; } = new List<string>();

        public MockupExceptionsOptions Exceptions { get; set; } = new MockupExceptionsOptions();

        public string Disclaimer { get; set; } = "This email was originally destined to the following recipents, and was mocked up because it was sent from a test environment.";
    }
}
