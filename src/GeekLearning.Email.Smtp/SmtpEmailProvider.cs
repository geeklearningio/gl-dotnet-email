namespace GeekLearning.Email.Smtp
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SmtpEmailProvider : IEmailProvider
    {
        private readonly string host;
        private readonly int port;
        private readonly string username;
        private readonly string password;

        public SmtpEmailProvider(IEmailProviderOptions options)
        {
            this.host = options.Parameters["Host"];
            if (string.IsNullOrWhiteSpace(this.host))
            {
                throw new ArgumentNullException("Host");
            }

            var portString = options.Parameters["Port"];
            if (string.IsNullOrWhiteSpace(portString) || !int.TryParse(portString, out this.port))
            {
                throw new ArgumentException("Port");
            }

            this.username = options.Parameters["UserName"];
            this.password = options.Parameters["Password"];
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

        public Task SendEmailAsync(
            IEmailAddress from,
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
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(from.DisplayName, from.Email));

            if(replyTo != null)
            {
                message.ReplyTo.Add(new MailboxAddress(replyTo.DisplayName, replyTo.Email));
            }
            
            foreach (var recipient in recipients)
            {
                message.To.Add(new MailboxAddress(recipient.DisplayName, recipient.Email));
            }

            foreach (var recipient in ccRecipients)
            {
                message.Cc.Add(new MailboxAddress(recipient.DisplayName, recipient.Email));
            }

            foreach (var recipient in bccRecipients)
            {
                message.Bcc.Add(new MailboxAddress(recipient.DisplayName, recipient.Email));
            }

            message.Subject = subject;

            var builder = new BodyBuilder
            {
                TextBody = text,
                HtmlBody = html
            };

            foreach (var attachment in attachments)
            {
                builder.Attachments.Add(attachment.FileName, attachment.Data, new ContentType(attachment.MediaType, attachment.MediaSubtype));
            }

            message.Body = builder.ToMessageBody();

            foreach (var textBodyPart in message.BodyParts.OfType<TextPart>())
            {
                textBodyPart.ContentTransferEncoding = ContentEncoding.Base64;
            }

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(this.host, this.port, SecureSocketOptions.None);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                if (!string.IsNullOrWhiteSpace(this.username))
                {
                    await client.AuthenticateAsync(this.username, this.password);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
