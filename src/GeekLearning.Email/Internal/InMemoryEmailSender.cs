namespace GeekLearning.Email.Internal
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class InMemoryEmailSender : IEmailSender
    {
        public List<TestEmail> SentEmails { get; private set; } = new List<TestEmail>();

        public Task SendEmailAsync(string subject, string message, params IEmailAddress[] to)
        {
            this.SentEmails.Add(new TestEmail
            {
                Subject = subject,
                Message = message,
                To = to
            });

            return Task.CompletedTask;
        }

        public Task SendEmailAsync(IEmailAddress from, string subject, string message, params IEmailAddress[] to)
        {
            this.SentEmails.Add(new TestEmail
            {
                From = from,
                Subject = subject,
                Message = message,
                To = to
            });

            return Task.CompletedTask;
        }

        public Task SendTemplatedEmail<T>(string templateKey, T context, params IEmailAddress[] to)
        {
            this.SentEmails.Add(new TestEmail
            {
                TemplateKey = templateKey,
                Context = context,
                To = to
            });

            return Task.CompletedTask;
        }

        public Task SendTemplatedEmail<T>(IEmailAddress from, string templateKey, T context, params IEmailAddress[] to)
        {
            this.SentEmails.Add(new TestEmail
            {
                From = from,
                TemplateKey = templateKey,
                Context = context,
                To = to
            });

            return Task.CompletedTask;
        }
    }

    public class TestEmail
    {
        public string Subject { get; set; }

        public string Message { get; set; }

        public IEmailAddress[] To { get; set; }

        public IEmailAddress From { get; set; }

        public string TemplateKey { get; set; }

        public object Context { get; set; }
    }
}
