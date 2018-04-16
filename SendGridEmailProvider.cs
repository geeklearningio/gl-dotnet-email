namespace GeekLearning.Email.SendGrid
{
    using global::SendGrid;
    using global::SendGrid.Helpers.Mail;
    using MimeKit;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
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

  /*      public async Task SendEmailAsync(
            IEmailAddress from,
            IEnumerable<IEmailAddress> recipients,
            string subject,
            string text,
            string html, List<IEmailAddress> replyTo, MimeKit.AttachmentCollection attachments)
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

            var response = await client.SendEmailAsync(message);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Cannot Send Email: {response.StatusCode}");
            }
        }  */

        public async Task SendEmailAsync(IEmailAddress from, IEnumerable<IEmailAddress> recipients, string subject, string bodyText, string bodyHtml, AttachmentCollection attachments = null)
        {
            var client = new SendGridClient(this.apiKey);

            SendGridMessage message;

            if (recipients.Count() == 1)
            {
                message = MailHelper.CreateSingleEmail(from.ToSendGridEmail(), recipients.First().ToSendGridEmail(), subject, bodyText, bodyHtml);
            }
            else
            {
                message = MailHelper.CreateSingleEmailToMultipleRecipients(
                    from.ToSendGridEmail(),
                    recipients.Select(email => email.ToSendGridEmail()).ToList(),
                    subject,
                    bodyText,
                    bodyHtml);
            }

            var response = await client.SendEmailAsync(message);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Cannot Send Email: {response.StatusCode}");
            }
        }
    }
}
