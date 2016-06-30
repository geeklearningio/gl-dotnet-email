namespace GeekLearning.Email
{
    using System.Collections.Generic;
    using System.Linq;

    public class EmailOptions
    {
        public string Key { get; set; }

        public string User { get; set; }

        public Internal.EmailAddress DefaultSender { get; set; }

        public string TemplateStorage { get; set; }

        public MockupOptions Mockup { get; set; } = new MockupOptions();

        public bool IsMockedUp
        {
            get
            {
                return this.Mockup.Recipients.Any()
                    && !string.IsNullOrEmpty(this.Mockup.Recipients.First());
            }
        }

        public class MockupOptions
        {
            public List<string> Recipients { get; set; } = new List<string>();

            public MockupExceptionsOptions Exceptions { get; set; } = new MockupExceptionsOptions();

            public string Disclaimer { get; set; } = "This email was originally destined to the following recipents, and was mocked up because it was sent from a test environment.";
        }

        public class MockupExceptionsOptions
        {
            public List<string> Emails { get; set; } = new List<string>();

            public List<string> Domains { get; set; } = new List<string>();
        }
    }
}
