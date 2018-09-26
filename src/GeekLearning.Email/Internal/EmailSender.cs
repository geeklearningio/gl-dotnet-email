﻿namespace GeekLearning.Email.Internal
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

        public Task SendEmailAsync(string subject, string message,  MimeKit.AttachmentCollection attachments, params IEmailAddress[] to)
        {
            return this.SendEmailAsync(options.DefaultSender, subject, message, attachments, to);
        }

        public Task SendEmailAsync(string subject, string message, params IEmailAddress[] to)
        {
            return this.SendEmailAsync(options.DefaultSender, subject, message, null, to);
        }

        public Task SendEmailAsync(IEmailAddress from, string subject, string message, params IEmailAddress[] to)
        {
            return this.SendEmailAsync(from, subject, message, null, to);
        }

        public Task SendEmailAsync(IEmailAddress from, string subject, string message, MimeKit.AttachmentCollection attachments, params IEmailAddress[] to)
        {
            return DoMockupAndSendEmailAsync(
                from,
                to,
                subject,
                message,
                string.Format("<html><header></header><body>{0}</body></html>", message), attachments);
        }

        public Task SendTemplatedEmailAsync<T>(string templateKey, T context,  MimeKit.AttachmentCollection attachments, params IEmailAddress[] to)
        {
            return this.SendTemplatedEmailAsync(options.DefaultSender, templateKey, context, attachments, to);
        }

        public Task SendTemplatedEmailAsync<T>(string templateKey, T context, params IEmailAddress[] to)
        {
            return this.SendTemplatedEmailAsync(options.DefaultSender, templateKey, context, to);
        }

        public async Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, params IEmailAddress[] to)
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
                null
                );
        }

        public async Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, MimeKit.AttachmentCollection attachments, params IEmailAddress[] to)
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
                attachments
                );
        }

        private Task<ITemplate> GetTemplateAsync(string templateKey, EmailTemplateType templateType)
        {
            return this.templateLoader.GetTemplate($"{templateKey}-{templateType}");
        }

        private async Task DoMockupAndSendEmailAsync(
            IEmailAddress from,
            IEmailAddress [] recipients,
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
 
                    var emailParts = trimmedEmail.Split('@');
                    var domain = emailParts[1];

                    if (!this.options.Mockup.Exceptions.Emails.Contains(trimmedEmail)
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

            await this.provider.SendEmailAsync(
                from,
                finalRecipients,
                subject,
                text,
                html, attachments);
        }
    }
}
