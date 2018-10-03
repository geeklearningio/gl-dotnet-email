namespace GeekLearning.Email.Internal
{
    using Microsoft.Extensions.Options;
    using Storage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Templating;

    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions options;
        private readonly IEmailProvider provider;
        private readonly ITemplateLoader templateLoader;

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

        public Task SendEmailAsync(string subject, string message, params IEmailAddress[] to)
        {
            return this.SendEmailAsync(options.DefaultSender, subject, message, to);
        }

        public Task SendEmailAsync(IEmailAddress from, string subject, string message, params IEmailAddress[] to)
        {
            return this.SendEmailAsync(options.DefaultSender, subject, message, Enumerable.Empty<IEmailAttachment>(), to);
        }

        public Task SendEmailAsync(IEmailAddress from, string subject, string message, IEnumerable<IEmailAttachment> attachments, params IEmailAddress[] to)
        {
            return this.SendEmailAsync(from, subject, message, attachments, to.ToArray(), new IEmailAddress[0], new IEmailAddress[0]);
        }

        public Task SendEmailAsync(IEmailAddress from, string subject, string message, IEnumerable<IEmailAttachment> attachments, IEmailAddress[] to, IEmailAddress[] cc, IEmailAddress[] bcc)
        {
            return DoMockupAndSendEmailAsync(
              from,
              to,
              cc,
              bcc,
              subject,
              message,
              string.Format("<html><header></header><body>{0}</body></html>", message),
              attachments);
        }

        public Task SendTemplatedEmailAsync<T>(string templateKey, T context, params IEmailAddress[] to)
        {
            return this.SendTemplatedEmailAsync(options.DefaultSender, templateKey, context, to);
        }

        public Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, params IEmailAddress[] to)
        {
            return this.SendTemplatedEmailAsync(from, templateKey, context, Enumerable.Empty<IEmailAttachment>(), to);
        }

        public Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, IEnumerable<IEmailAttachment> attachments, params IEmailAddress[] to)
        {
            return this.SendTemplatedEmailAsync(from, templateKey, context, attachments, to, new IEmailAddress[0], new IEmailAddress[0]);
        }

        public async Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, IEnumerable<IEmailAttachment> attachments, IEmailAddress[] to, IEmailAddress[] cc, IEmailAddress[] bcc)
        {
            var subjectTemplate = await this.GetTemplateAsync(templateKey, EmailTemplateType.Subject);
            var textTemplate = await this.GetTemplateAsync(templateKey, EmailTemplateType.BodyText);
            var htmlTemplate = await this.GetTemplateAsync(templateKey, EmailTemplateType.BodyHtml);

            await this.DoMockupAndSendEmailAsync(
                from,
                to,
                cc,
                bcc,
                subjectTemplate.Apply(context),
                textTemplate.Apply(context),
                htmlTemplate.Apply(context),
                attachments);
        }

        protected virtual Task<ITemplate> GetTemplateAsync(string templateKey, EmailTemplateType templateType)
        {
            return this.templateLoader.GetTemplate($"{templateKey}-{templateType}");
        }

        private IEnumerable<IEmailAddress> MockRecipients(IEnumerable<IEmailAddress> recipients, ICollection<IEmailAddress> alreadyMockedUpRecipients)
        {
            var finalRecipients = new List<IEmailAddress>();
            if (this.options.Mockup.Recipients.Any() && !string.IsNullOrEmpty(this.options.Mockup.Recipients.First()))
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
                        if (!alreadyMockedUpRecipients.Any())
                        {
                            foreach (var mockupRecipient in this.options.Mockup.Recipients)
                            {
                                finalRecipients.Add(new EmailAddress(mockupRecipient, "Mockup Recipient"));
                            }
                        }

                        if (!alreadyMockedUpRecipients.Any(a => a.DisplayName == recipient.DisplayName && a.Email == recipient.Email))
                        {
                            alreadyMockedUpRecipients.Add(recipient);
                        }
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

            return finalRecipients;
        }

        private async Task DoMockupAndSendEmailAsync(
            IEmailAddress from,
            IEnumerable<IEmailAddress> recipients,
            IEnumerable<IEmailAddress> ccRecipients,
            IEnumerable<IEmailAddress> bccRecipients,
            string subject,
            string text,
            string html,
            IEnumerable<IEmailAttachment> attachments)
        {
            var mockedUpRecipients = new List<IEmailAddress>();

            var finalToRecipients = MockRecipients(recipients, mockedUpRecipients);
            var finalCcRecipients = MockRecipients(ccRecipients, mockedUpRecipients);
            var finalBccRecipients = MockRecipients(bccRecipients, mockedUpRecipients);

            if (mockedUpRecipients.Any())
            {
                var disclaimer = this.options.Mockup.Disclaimer;
                var joinedMockedUpRecipients = string.Join(", ", mockedUpRecipients.Select(r => $"{r.DisplayName} ({r.Email})"));

                text = string.Concat(text, Environment.NewLine, disclaimer, Environment.NewLine, joinedMockedUpRecipients);
                html = string.Concat(html, "<br/><i>", disclaimer, "<br/>", joinedMockedUpRecipients, "</i>");
            }

            await this.provider.SendEmailAsync(
                from,
                finalToRecipients,
                finalCcRecipients,
                finalBccRecipients,
                subject,
                text,
                html,
                attachments);
        }
    }
}
