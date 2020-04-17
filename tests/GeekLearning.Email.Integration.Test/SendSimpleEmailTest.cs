namespace GeekLearning.Email.Integration.Test
{
    using GeekLearning.Email.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Storage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Templating;
    using Xunit;

    [Collection(nameof(IntegrationCollection))]
    [Trait("Operation", "SendSimpleEmail"), Trait("Kind", "Integration")]
    public class SendSimpleEmailTest
    {
        readonly StoresFixture storeFixture;

        public SendSimpleEmailTest(StoresFixture fixture)
        {
            this.storeFixture = fixture;
        }

        [Fact(DisplayName = nameof(Send))]
        public async Task Send()
        {
            var options = Datas.GetOptions(storeFixture);

            var providerTypes = new List<IEmailProviderType>
            {
                new SendGrid.SendGridEmailProviderType(),
            };

            var emailSender = new Internal.EmailSender(
                providerTypes,
                options,
                this.storeFixture.Services.GetRequiredService<IStorageFactory>(),
                this.storeFixture.Services.GetRequiredService<ITemplateLoaderFactory>());

            await emailSender.SendEmailAsync("Simple mail", "Hello, it's a simple mail", new Internal.EmailAddress
            {
                DisplayName = "test user",
                Email = "annayafi@gmail.com"
            });
        }

        [Fact(DisplayName = nameof(SendWithReplyTo))]
        public async Task SendWithReplyTo()
        {
            var options = Datas.GetOptions(storeFixture);

            var providerTypes = new List<IEmailProviderType>
            {
                new SendGrid.SendGridEmailProviderType(),
            };

            var emailSender = new Internal.EmailSender(
                providerTypes,
                options,
                this.storeFixture.Services.GetRequiredService<IStorageFactory>(),
                this.storeFixture.Services.GetRequiredService<ITemplateLoaderFactory>());

            await emailSender.SendEmailAsync(
                new EmailAddress
                {
                    DisplayName = "Sender user test replyTo",
                    Email = "no-reply@test.geeklearning.io"
                },
                new EmailAddress
                {
                    DisplayName = "Reply Address",
                    Email = "no-reply2@test.geeklearning.io"
                },
                "Simple mail", "Hello, it's a simple mail", 
                new Internal.EmailAddress
                {
                    DisplayName = "test user",
                    Email = "test@test.geeklearning.io"
                });
        }

        [Fact(DisplayName = nameof(SendWithCC))]
        public async Task SendWithCC()
        {
            var options = Datas.GetOptions(storeFixture);

            var providerTypes = new List<IEmailProviderType>
            {
                new SendGrid.SendGridEmailProviderType(),
            };

            var emailSender = new EmailSender(
                providerTypes,
                options,
                this.storeFixture.Services.GetRequiredService<IStorageFactory>(),
                this.storeFixture.Services.GetRequiredService<ITemplateLoaderFactory>());


            await emailSender.SendEmailAsync(new EmailAddress
            {
                DisplayName = "Sender user test cc",
                Email = "no-reply@test.geeklearning.io"
            },
            "Cc test", "Hello, this is an email with cc recipients",
            Enumerable.Empty<IEmailAttachment>(),
            new EmailAddress
            {
                DisplayName = "recipient user",
                Email = Datas.FirstRecipient
            }.Yield(),
            new EmailAddress
            {
                DisplayName = "cc user",
                Email = Datas.SecondRecipient
            }.Yield(),
            new IEmailAddress[0]);
        }

        [Fact(DisplayName = nameof(SendWithBcc))]
        public async Task SendWithBcc()
        {
            var options = Datas.GetOptions(storeFixture);

            var providerTypes = new List<IEmailProviderType>
            {
                new SendGrid.SendGridEmailProviderType(),
            };

            var emailSender = new Internal.EmailSender(
                providerTypes,
                options,
                this.storeFixture.Services.GetRequiredService<IStorageFactory>(),
                this.storeFixture.Services.GetRequiredService<ITemplateLoaderFactory>());


            await emailSender.SendEmailAsync(new Internal.EmailAddress
            {
                DisplayName = "Sender user test bcc",
                Email = "no-reply@test.geeklearning.io"
            },
            "Bcc test", "Hello, this is an email with bcc recipients",
            Enumerable.Empty<IEmailAttachment>(),
            new Internal.EmailAddress
            {
                DisplayName = "recipient user",
                Email = Datas.FirstRecipient
            }.Yield(),
            new IEmailAddress[0],
            new Internal.EmailAddress
            {
                DisplayName = "test user",
                Email = Datas.SecondRecipient
            }.Yield());
        }

        [Fact(DisplayName = nameof(SendWithAttachments))]
        public async Task SendWithAttachments()
        {
            var options = Datas.GetOptions(storeFixture);

            var providerTypes = new List<IEmailProviderType>
            {
                new SendGrid.SendGridEmailProviderType(),
            };

            var emailSender = new Internal.EmailSender(
                providerTypes,
                options,
                this.storeFixture.Services.GetRequiredService<IStorageFactory>(),
                this.storeFixture.Services.GetRequiredService<ITemplateLoaderFactory>());

            var data = System.IO.File.ReadAllBytes(@"Files\beach.jpeg");
            var image = new EmailAttachment("Beach.jpeg", data, "image", "jpeg");

            data = System.IO.File.ReadAllBytes(@"Files\sample.pdf");
            var pdf = new EmailAttachment("Sample.pdf", data, "application", "pdf");

            await emailSender.SendEmailAsync(new Internal.EmailAddress
            {
                DisplayName = "test user attachm ments",
                Email = "no-reply@test.geeklearning.io"
            }, "Test mail with attachments", "Hello, this is an email with attachments", new List<IEmailAttachment> { image, pdf }, new Internal.EmailAddress
            {
                DisplayName = "test user",
                Email = Datas.FirstRecipient
            });
        }

        [Fact(DisplayName = nameof(ErrorSendWithCCDuplicates))]
        public async Task ErrorSendWithCCDuplicates()
        {
            var options = Datas.GetOptions(storeFixture);

            var providerTypes = new List<IEmailProviderType>
            {
                new SendGrid.SendGridEmailProviderType(),
            };

            var emailSender = new Internal.EmailSender(
                providerTypes,
                options,
                this.storeFixture.Services.GetRequiredService<IStorageFactory>(),
                this.storeFixture.Services.GetRequiredService<ITemplateLoaderFactory>());

            await Assert.ThrowsAsync<ArgumentException>(() =>
                 emailSender.SendEmailAsync(new Internal.EmailAddress
                 {
                     DisplayName = "Sender user test cc",
                     Email = "no-reply@test.geeklearning.io"
                 },
                "Cc test", "Hello, this is an email with cc recipients",
                Enumerable.Empty<IEmailAttachment>(),
                new Internal.EmailAddress
                {
                    DisplayName = "recipient user",
                    Email = Datas.SecondRecipient
                }.Yield(),
                new Internal.EmailAddress
                {
                    DisplayName = "cc user",
                    Email = Datas.SecondRecipient
                }.Yield(),
                new IEmailAddress[0]));
        }
    }
}
