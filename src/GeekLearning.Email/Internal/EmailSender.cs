namespace GeekLearning.Email.Internal
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Storage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Templating;

    public class EmailSender : IEmailSender
    {
        private EmailOptions options;
        private ITemplateLoader templateLoader;

        public EmailSender(IOptions<EmailOptions> options, ITemplateLoaderFactory templateLoaderFactory, IStorageFactory storageFactory)
        {
            this.options = options.Value;
            this.templateLoader = templateLoaderFactory.Create(storageFactory.GetStore(options.Value.TemplateStorage));
        }

        public Task SendEmailAsync(string subject, string message, params IEmailAddress[] to)
        {
            return SendEmailAsync(options.DefaultSender, subject, message, to);
        }

        public Task SendEmailAsync(IEmailAddress from, string subject, string message, params IEmailAddress[] to)
        {
            return SendGridSend(
                options.User,
                options.Key,
                from,
                to,
                subject,
                message,
                string.Format("<html><header></header><body>{0}</body></html>", message));
        }

        public Task SendTemplatedEmail<T>(string templateKey, T context, params IEmailAddress[] to)
        {
            return SendTemplatedEmail(options.DefaultSender, templateKey, context, to);
        }

        public async Task SendTemplatedEmail<T>(IEmailAddress from, string templateKey, T context, params IEmailAddress[] to)
        {
            var subjectTemplate = await this.GetTemplateAsync(templateKey, EmailTemplateType.Subject);
            var textTemplate = await this.GetTemplateAsync(templateKey, EmailTemplateType.BodyText);
            var htmlTemplate = await this.GetTemplateAsync(templateKey, EmailTemplateType.BodyHtml);

            await this.SendGridSend(
                options.User,
                options.Key,
                from,
                to,
                subjectTemplate.Apply(context),
                textTemplate.Apply(context),
                htmlTemplate.Apply(context));
        }

        private async Task SendGridSend(
            string apiUser,
            string apiKey,
            IEmailAddress from,
            IEnumerable<IEmailAddress> recipients,
            string subject,
            string text,
            string html)
        {
            var finalRecipients = new List<IEmailAddress>();
            var mockedUpRecipients = new List<IEmailAddress>();
            if (this.options.IsMockedUp)
            {
                foreach (var recipient in recipients)
                {
                    var emailParts = recipient.Email.Split('@');
                    if (emailParts.Length != 2)
                    {
                        throw new NotSupportedException("Bad recipient email.");
                    }

                    var domain = emailParts[1];

                    if (!this.options.Mockup.Exceptions.Emails.Contains(recipient.Email)
                        && !this.options.Mockup.Exceptions.Domains.Contains(domain))
                    {
                        if (!mockedUpRecipients.Any())
                        {
                            foreach (var mockupRecipient in this.options.Mockup.Recipients)
                            {
                                finalRecipients.Add(new EmailAddress(mockupRecipient, "Mockup Recipient"));
                            }
                        }

                        mockedUpRecipients.Add(recipient);
                    }
                    else
                    {
                        finalRecipients.Add(recipient);
                    }
                }
            }
            else
            {
                finalRecipients = recipients.ToList();
            }

            if (mockedUpRecipients.Any())
            {
                var disclaimer = this.options.Mockup.Disclaimer;
                var joinedMockedUpRecipients = string.Join(", ", mockedUpRecipients.Select(r => $"{r.DisplayName} ({r.Email})"));

                text = string.Concat(text, Environment.NewLine, disclaimer, Environment.NewLine, joinedMockedUpRecipients);
                html = string.Concat(html, "<br/><i>", disclaimer, "<br/>", joinedMockedUpRecipients, "</i>");
            }

            var client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/api/mail.send.json");

            var variables = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("api_user", apiUser),
                new KeyValuePair<string, string>("api_key", apiKey),
                new KeyValuePair<string, string>("from", from.Email),
                new KeyValuePair<string, string>("fromname", from.DisplayName),
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("text", text),
                new KeyValuePair<string, string>("html", html),
            };

            if (finalRecipients.Count() == 1)
            {
                variables.Add(new KeyValuePair<string, string>("to", finalRecipients.First().Email));
                variables.Add(new KeyValuePair<string, string>("toname", finalRecipients.First().DisplayName));
            }
            else
            {
                foreach (var recipient in finalRecipients)
                {
                    variables.Add(new KeyValuePair<string, string>("to[]", recipient.Email));
                    variables.Add(new KeyValuePair<string, string>("toname[]", recipient.DisplayName));
                }
            }

            message.Content = new FormUrlEncodedContent(variables);

            var response = await client.SendAsync(message);
            var responseBody = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Cannot Send Email");
            }
        }

        private Task<ITemplate> GetTemplateAsync(string templateKey, EmailTemplateType templateType)
        {
            return this.templateLoader.GetTemplate($"{templateKey}-{templateType}");
        }
    }
}
