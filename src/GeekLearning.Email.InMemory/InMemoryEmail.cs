namespace GeekLearning.Email.InMemory
{
    public class InMemoryEmail
    {
        public string Subject { get; set; }

        public string MessageText { get; set; }

        public string MessageHtml { get; set; }

        public IEmailAddress[] To { get; set; }

        public IEmailAddress From { get; set; }
    }
}
