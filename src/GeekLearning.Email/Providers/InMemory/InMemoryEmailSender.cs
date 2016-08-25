namespace GeekLearning.Email.Providers.InMemory
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class InMemoryEmailSender : IEmailSender
    {
        public IList<InMemoryEmail> SentEmails { get; private set; } = new List<InMemoryEmail>();

        public Task SendEmailAsync(string subject, string message, params IEmailAddress[] to)
        {
            this.SentEmails.Add(new InMemoryEmail
            {
                Subject = subject,
                Message = message,
                To = to
            });

            return Task.CompletedTask;
        }

        public Task SendEmailAsync(IEmailAddress from, string subject, string message, params IEmailAddress[] to)
        {
            this.SentEmails.Add(new InMemoryEmail
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
            this.SentEmails.Add(new InMemoryEmail
            {
                TemplateKey = templateKey,
                Context = context,
                To = to
            });

            return Task.CompletedTask;
        }

        public Task SendTemplatedEmail<T>(IEmailAddress from, string templateKey, T context, params IEmailAddress[] to)
        {
            this.SentEmails.Add(new InMemoryEmail
            {
                From = from,
                TemplateKey = templateKey,
                Context = context,
                To = to
            });

            return Task.CompletedTask;
        }
    }
}
