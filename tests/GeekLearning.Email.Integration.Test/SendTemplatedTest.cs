using GeekLearning.Email.Providers.SendGrid;
using GeekLearning.Storage;
using GeekLearning.Templating;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GeekLearning.Email.Integration.Test
{
    [Collection(nameof(IntegrationCollection))]
    [Trait("Operation", "SendTemplated"), Trait("Kind", "Integration")]
    public class SendTemplatedTest
    {
        StoresFixture storeFixture;

        public SendTemplatedTest(StoresFixture fixture)
        {
            this.storeFixture = fixture;
        }

        [Theory(DisplayName = nameof(SendNotification1)), InlineData("azure"), InlineData("filesystem")]
        public async Task SendNotification1(string storeName)
        {
            var options = Microsoft.Extensions.Options.Options.Create(new EmailOptions
            {
                TemplateStorage = storeName,
                DefaultSender = new Internal.EmailAddress
                {
                    DisplayName = "test user",
                    Email = "no-reply@test.geeklearning.io"
                },
                Key = storeFixture.SendGridKey,
                User = storeFixture.SendGridUser,
                Mockup = new EmailOptions.MockupOptions
                {
                    Disclaimer = "",
                    Exceptions = new EmailOptions.MockupExceptionsOptions
                    {
                        Domains = new List<string>(),
                        Emails = new List<string>(),
                    },
                    Recipients = new List<string>()
                }
            });
            var emailSender = new SendGridEmailSender(options,
                this.storeFixture.Services.GetRequiredService<ITemplateLoaderFactory>(),
                this.storeFixture.Services.GetRequiredService<IStorageFactory>()
                );

            await emailSender.SendTemplatedEmail("Notification1", new { }, new Internal.EmailAddress
            {
                DisplayName = "test user",
                Email = "no-reply@test.geeklearning.io"
            });
        }
    }
}
