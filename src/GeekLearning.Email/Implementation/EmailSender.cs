namespace GeekLearning.Email.Implementation
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
            var client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/api/mail.send.json");

            var variables = new Dictionary<string, string>()
            {
                ["api_user"] = apiUser,
                ["api_key"] = apiKey,
                ["from"] = from.Email,
                ["fromname"] = from.DisplayName,
                ["subject"] = subject,
                ["text"] = text,
                ["html"] = html,
            };

            if (recipients.Count() == 1)
            {
                variables["to"] = string.IsNullOrEmpty(this.options.MockupRecipient) ? recipients.First().Email : this.options.MockupRecipient;
                variables["toname"] = recipients.First().DisplayName;
            }
            else
            {
                foreach (var recipient in recipients)
                {
                    variables["to[]"] = string.IsNullOrEmpty(this.options.MockupRecipient) ? recipient.Email : this.options.MockupRecipient;
                    variables["toname[]"] = recipient.DisplayName;
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
