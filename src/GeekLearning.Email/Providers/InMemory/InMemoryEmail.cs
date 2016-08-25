namespace GeekLearning.Email.Providers.InMemory
{
    public class InMemoryEmail
    {
        public string Subject { get; set; }

        public string Message { get; set; }

        public IEmailAddress[] To { get; set; }

        public IEmailAddress From { get; set; }

        public string TemplateKey { get; set; }

        public object Context { get; set; }
    }
}
