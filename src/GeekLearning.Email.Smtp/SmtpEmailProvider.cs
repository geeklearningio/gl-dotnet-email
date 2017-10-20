namespace GeekLearning.Email.Smtp
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;
    using System;
    using System.Collections.Generic;
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

        public async Task SendEmailAsync(
            IEmailAddress from,
            IEnumerable<IEmailAddress> recipients,
            string subject,
            string text,
            string html)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(from.DisplayName, from.Email));
            foreach (var recipient in recipients)
            {
                message.To.Add(new MailboxAddress(recipient.DisplayName, recipient.Email));
            }

            message.Subject = subject;

            var builder = new BodyBuilder
            {
                TextBody = text,
                HtmlBody = html
            };

            message.Body = builder.ToMessageBody();

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
