namespace GeekLearning.Email.Integration.Test
{
    using GeekLearning.Email.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Storage;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Templating;
    using Xunit;

    [Collection(nameof(IntegrationCollection))]
    [Trait("Operation", "SendTemplated"), Trait("Kind", "Integration")]
    public class SendTemplatedTest
    {
        readonly StoresFixture storeFixture;

        public SendTemplatedTest(StoresFixture fixture)
        {
            this.storeFixture = fixture;
        }

        [Theory(DisplayName = nameof(SendNotification1)), InlineData("azure"), InlineData("filesystem")]
        public async Task SendNotification1(string storeName)
        {
            var options = Datas.GetOptions(storeFixture, storeName);

            var providerTypes = new List<IEmailProviderType>
            {
                new SendGrid.SendGridEmailProviderType(),
            };

            var emailSender = new EmailSender(
                providerTypes,
                options,
                this.storeFixture.Services.GetRequiredService<IStorageFactory>(),
                this.storeFixture.Services.GetRequiredService<ITemplateLoaderFactory>());

            await emailSender.SendTemplatedEmailAsync("Notification1", new { }, new Internal.EmailAddress
            {
                DisplayName = "test user",
                Email = Datas.FirstRecipient
            });
        }

        [Theory(DisplayName = nameof(SendNotificationWithWithCC)), InlineData("filesystem"), InlineData("azure")]
        public async Task SendNotificationWithWithCC(string storeName)
        {
            var options = Datas.GetOptions(storeFixture, storeName);

            var providerTypes = new List<IEmailProviderType>
            {
                new SendGrid.SendGridEmailProviderType(),
            };

            var emailSender = new EmailSender(
                providerTypes,
                options,
                this.storeFixture.Services.GetRequiredService<IStorageFactory>(),
                this.storeFixture.Services.GetRequiredService<ITemplateLoaderFactory>());

            await emailSender.SendTemplatedEmailAsync(new EmailAddress
            {
                DisplayName = "Sender user test cc",
                Email = "no-reply@test.geeklearning.io"
            }, "Notification1", new { },
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

        [Theory(DisplayName = nameof(SendNotificationWithWithBbc)), InlineData("filesystem"), InlineData("azure")]
        public async Task SendNotificationWithWithBbc(string storeName)
        {
            var options = Datas.GetOptions(storeFixture, storeName);

            var providerTypes = new List<IEmailProviderType>
            {
                new SendGrid.SendGridEmailProviderType(),
            };

            var emailSender = new EmailSender(
                providerTypes,
                options,
                this.storeFixture.Services.GetRequiredService<IStorageFactory>(),
                this.storeFixture.Services.GetRequiredService<ITemplateLoaderFactory>());

            await emailSender.SendTemplatedEmailAsync(new EmailAddress
            {
                DisplayName = "Sender user test cc",
                Email = "no-reply@test.geeklearning.io"
            }, "Notification1", new { },
            Enumerable.Empty<IEmailAttachment>(),
            new EmailAddress
            {
                DisplayName = "recipient user",
                Email = Datas.FirstRecipient
            }.Yield(),
            new IEmailAddress[0],
            new EmailAddress
            {
                DisplayName = "test user",
                Email = Datas.SecondRecipient
            }.Yield());
        }

        [Theory(DisplayName = nameof(SendNotificationWithAttachments)), InlineData("filesystem"), InlineData("azure")]
        public async Task SendNotificationWithAttachments(string storeName)
        {
            var options = Datas.GetOptions(storeFixture, storeName);

            var providerTypes = new List<IEmailProviderType>
            {
                new SendGrid.SendGridEmailProviderType(),
            };

            var emailSender = new EmailSender(
                providerTypes,
                options,
                this.storeFixture.Services.GetRequiredService<IStorageFactory>(),
                this.storeFixture.Services.GetRequiredService<ITemplateLoaderFactory>());

            var data = System.IO.File.ReadAllBytes(@"Files\beach.jpeg");
            var image = new EmailAttachment("Beach.jpeg", data, "image", "jpeg");

            data = System.IO.File.ReadAllBytes(@"Files\sample.pdf");
            var pdf = new EmailAttachment("Sample.pdf", data, "application", "pdf");

            await emailSender.SendTemplatedEmailAsync(new EmailAddress
            {
                DisplayName = "test user attachm ments",
                Email = "no-reply@test.geeklearning.io"
            },
            "Notification1",
            new { },
            new List<IEmailAttachment> { image, pdf },
            new EmailAddress
            {
                DisplayName = "test user",
                Email = Datas.FirstRecipient
            });
        }
    }
}
