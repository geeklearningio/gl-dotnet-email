namespace GeekLearning.Email.InMemory
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class InMemoryEmailProvider : IEmailProvider
    {
        private readonly IInMemoryEmailRepository inMemoryEmailRepository;

        public InMemoryEmailProvider(IEmailProviderOptions options, IInMemoryEmailRepository inMemoryEmailRepository)
        {
            this.inMemoryEmailRepository = inMemoryEmailRepository;
        }

        public Task SendEmailAsync(IEmailAddress from, IEnumerable<IEmailAddress> recipients, string subject, string text, string html)
        {
            return SendEmailAsync(from, recipients, subject, text, html, Enumerable.Empty<IEmailAttachment>());
        }

        public Task SendEmailAsync(IEmailAddress from, IEnumerable<IEmailAddress> recipients, string subject, string text, string html, IEnumerable<IEmailAttachment> attachments)
        {
            return SendEmailAsync(from, recipients, Enumerable.Empty<IEmailAddress>(), Enumerable.Empty<IEmailAddress>(), subject, text, html, Enumerable.Empty<IEmailAttachment>());
        }

        public Task SendEmailAsync(IEmailAddress from, IEnumerable<IEmailAddress> recipients, IEnumerable<IEmailAddress> ccRecipients, IEnumerable<IEmailAddress> bccRecipients, string subject, string text, string html, IEnumerable<IEmailAttachment> attachments, IEmailAddress replyTo = null)
        {
            this.inMemoryEmailRepository.Save(new InMemoryEmail
            {
                Subject = subject,
                MessageText = text,
                MessageHtml = html,
                To = recipients.ToArray(),
                Cc = ccRecipients.ToArray(),
                Bcc = bccRecipients.ToArray(),
                From = from,
                replyTo = replyTo,
                Attachments = attachments
            });

            return Task.FromResult(0);
        }
    }
}
