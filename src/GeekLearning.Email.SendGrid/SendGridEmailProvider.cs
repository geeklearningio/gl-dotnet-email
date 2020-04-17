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

        public Task SendEmailAsync(IEmailAddress from,
            IEnumerable<IEmailAddress> recipients,
            string subject,
            string text,
            string html,
            IEnumerable<IEmailAttachment> attachments)
        {
            return SendEmailAsync(from, recipients, Enumerable.Empty<IEmailAddress>(), Enumerable.Empty<IEmailAddress>(), subject, text, html, Enumerable.Empty<IEmailAttachment>());
        }

        public async Task SendEmailAsync(IEmailAddress from, IEnumerable<IEmailAddress> recipients, IEnumerable<IEmailAddress> ccRecipients, IEnumerable<IEmailAddress> bccRecipients, string subject, string text, string html, IEnumerable<IEmailAttachment> attachments, IEmailAddress replyTo = null)
        {
            var allRecipients = new List<IEmailAddress>(recipients);
            allRecipients.AddRange(ccRecipients);
            allRecipients.AddRange(bccRecipients);

            if (allRecipients.GroupBy(r => r.Email).Count() < allRecipients.Count)
            {
                throw new ArgumentException("Each email address should be unique between to, cc, and bcc recipients. We found duplicates.");
            }

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

            foreach (var ccRecipient in ccRecipients)
            {
                message.AddCc(ccRecipient.Email, ccRecipient.DisplayName);
            }

            foreach (var bccRecipient in bccRecipients)
            {
                message.AddBcc(bccRecipient.Email, bccRecipient.DisplayName);
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

            if(replyTo != null)
            {
                message.ReplyTo = replyTo.ToSendGridEmail();
            }

            var response = await client.SendEmailAsync(message);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Cannot Send Email: {response.StatusCode}");
            }
        }
    }
}
