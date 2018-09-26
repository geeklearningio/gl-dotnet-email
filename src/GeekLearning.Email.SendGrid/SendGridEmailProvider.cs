namespace GeekLearning.Email.SendGrid
{
    using global::SendGrid;
    using global::SendGrid.Helpers.Mail;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SendGridEmailProvider : IEmailProvider
    {
        private readonly string apiKey;

        public SendGridEmailProvider(IEmailProviderOptions options)
        {
            this.apiKey = options.Parameters["Key"];

            if (string.IsNullOrWhiteSpace(this.apiKey))
            {
                throw new ArgumentNullException("apiKey");
            }
        }

        public Task SendEmailAsync(
            IEmailAddress from,
            IEnumerable<IEmailAddress> recipients,
            string subject,
            string text,
            string html)
        {
            return SendEmailAsync(from, recipients, subject, text, html, Enumerable.Empty<IEmailAttachment>());
        }

        public async Task SendEmailAsync(IEmailAddress from,
            IEnumerable<IEmailAddress> recipients,
            string subject,
            string text,
            string html,
            IEnumerable<IEmailAttachment> attachments)
        {
            var client = new SendGridClient(this.apiKey);

            SendGridMessage message;

            if (recipients.Count() == 1)
            {
                message = MailHelper.CreateSingleEmail(from.ToSendGridEmail(), recipients.First().ToSendGridEmail(), subject, text, html);
            }
            else
            {
                message = MailHelper.CreateSingleEmailToMultipleRecipients(
                    from.ToSendGridEmail(),
                    recipients.Select(email => email.ToSendGridEmail()).ToList(),
                    subject,
                    text,
                    html);
            }

            if (attachments.Any())
            {
                message.AddAttachments(attachments.Select(a => new Attachment
                {
                    Filename = a.FileName,
                    Type = a.ContentType,
                    Content = Convert.ToBase64String(a.Data)
                }).ToList());
            }

            var response = await client.SendEmailAsync(message);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Cannot Send Email: {response.StatusCode}");
            }
        }
    }
}
