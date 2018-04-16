namespace GeekLearning.Email.Internal
{
    using Microsoft.Extensions.Options;
    using Storage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Templating;
    using System.Text.RegularExpressions;

    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions options;
        private readonly IEmailProvider provider;
        private readonly ITemplateLoader templateLoader;
      //  private readonly ILogger logger;

        public EmailSender(
            IEnumerable<IEmailProviderType> emailProviderTypes,
            IOptions<EmailOptions> options,
            IStorageFactory storageFactory,
            ITemplateLoaderFactory templateLoaderFactory)
        {
            this.options = options.Value;

            var providerType = emailProviderTypes
                .FirstOrDefault(x => x.Name == this.options.Provider.Type);
            if (providerType == null)
            {
                throw new ArgumentNullException("ProviderType", $"The provider type {this.options.Provider.Type} does not exist. Maybe you are missing a reference or an Add method call in your Startup class.");
            }

            this.provider = providerType.BuildProvider(this.options.Provider);

            if (!string.IsNullOrWhiteSpace(this.options.TemplateStorage))
            {
                var store = storageFactory.GetStore(this.options.TemplateStorage);
                if (store == null)
                {
                    throw new ArgumentNullException("TemplateStorage", $"There is no file store configured with name {this.options.TemplateStorage}. Unable to initialize email templating.");
                }

                this.templateLoader = templateLoaderFactory.Create(store);
            }
        }

        public Task SendEmailAsync(string subject, string message, IEnumerable<IEmailAddress> to,  MimeKit.AttachmentCollection attachments)
        {
            return this.SendEmailAsync(options.DefaultSender, subject, message, to, attachments);
        }

        public Task SendEmailAsync(IEmailAddress from, string subject, string message, IEnumerable<IEmailAddress> to, MimeKit.AttachmentCollection attachments)
        {
            return DoMockupAndSendEmailAsync(
                from,
                to,
                subject,
                message,
                string.Format("<html><header></header><body>{0}</body></html>", message), attachments);
        }

        public Task SendTemplatedEmailAsync<T>(string templateKey, T context, IEnumerable<IEmailAddress> to,  MimeKit.AttachmentCollection attachments)
        {
            return this.SendTemplatedEmailAsync(options.DefaultSender, templateKey, context, to,  attachments);
        }

        public async Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, IEnumerable<IEmailAddress> to,  MimeKit.AttachmentCollection attachments)
        {
            var subjectTemplate = await this.GetTemplateAsync(templateKey, EmailTemplateType.Subject);
            var textTemplate = await this.GetTemplateAsync(templateKey, EmailTemplateType.BodyText);
            var htmlTemplate = await this.GetTemplateAsync(templateKey, EmailTemplateType.BodyHtml);

            await this.DoMockupAndSendEmailAsync(
                from,
                to,
                subjectTemplate.Apply(context),
                textTemplate.Apply(context),
                htmlTemplate.Apply(context),
                attachments);
        }

        private Task<ITemplate> GetTemplateAsync(string templateKey, EmailTemplateType templateType)
        {
           // logger.LogInformation(string.Format("EmailSender: GetTemplateAsync (key: {0} type: {1})", templateKey, templateType.ToString()));
            return this.templateLoader.GetTemplate($"{templateKey}-{templateType}");
        }

        private async Task DoMockupAndSendEmailAsync(
            IEmailAddress from,
            IEnumerable<IEmailAddress> recipients,
            string subject,
            string text,
            string html,
            MimeKit.AttachmentCollection attachments)
        {
            var finalRecipients = new List<IEmailAddress>();
            var mockedUpRecipients = new List<IEmailAddress>();

            if (options.Mockup.Recipients.Any() && !string.IsNullOrEmpty(options.Mockup.Recipients.First()))
            {
                foreach (var recipient in recipients)
                {
                    string trimmedEmail = recipient.Email.Trim();
                    if (Regex.IsMatch(trimmedEmail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase).Equals(false))
                    {
                        throw new NotSupportedException("Bad recipient email.");
                    }

                    var emailParts = trimmedEmail.Split('@');
                    var domain = emailParts[1];

                    if (!this.options.Mockup.Exceptions.Emails.Contains(recipient.Email)
                        && !this.options.Mockup.Exceptions.Domains.Contains(domain))
                    {
                        if (!mockedUpRecipients.Any())
                        {
                            foreach (var mockupRecipient in this.options.Mockup.Recipients)
                            {
                                finalRecipients.Add(new EmailAddress(mockupRecipient, "Mockup Recipient", AddressTarget.To));
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

            await this.provider.SendEmailAsync(
                from,
                finalRecipients,
                subject,
                text,
                html, attachments);
        }


    }
}
