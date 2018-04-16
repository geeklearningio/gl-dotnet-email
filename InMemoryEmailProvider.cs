namespace GeekLearning.Email.InMemory
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class InMemoryEmailProvider : IEmailProvider
    {
        private IInMemoryEmailRepository inMemoryEmailRepository;

        public InMemoryEmailProvider(IEmailProviderOptions options, IInMemoryEmailRepository inMemoryEmailRepository)
        {
            this.inMemoryEmailRepository = inMemoryEmailRepository;
        }

        public Task SendEmailAsync(IEmailAddress from, IEnumerable<IEmailAddress> recipients, string subject, string text, string html, MimeKit.AttachmentCollection attachments)
        {
            this.inMemoryEmailRepository.Save(new InMemoryEmail
            {
                Subject = subject,
                MessageText = text,
                MessageHtml = html,
                To = recipients.ToArray(),
                From = from
            });

            return Task.FromResult(0);
        }
    }
}
