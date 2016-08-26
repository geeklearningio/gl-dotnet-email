namespace GeekLearning.Email.InMemory
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class InMemoryEmailProvider : IEmailProvider
    {
        public InMemoryEmailProvider(IEmailProviderOptions options)
        {
        }

        public IList<InMemoryEmail> SentEmails { get; private set; } = new List<InMemoryEmail>();

        public Task SendEmailAsync(IEmailAddress from, IEnumerable<IEmailAddress> recipients, string subject, string text, string html)
        {
            this.SentEmails.Add(new InMemoryEmail
            {
                Subject = subject,
                MessageText = text,
                MessageHtml = html,
                To = recipients.ToArray(),
                From = from,
            });

            return Task.CompletedTask;
        }
    }
}
