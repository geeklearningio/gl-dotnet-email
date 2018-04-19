namespace GeekLearning.Email.Integration.Test
{
    using Microsoft.Extensions.DependencyInjection;
    using Storage;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Templating;
    using Xunit;

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
                Provider = new EmailProviderOptions
                {
                    Type = "SendGrid",
                    Parameters = new Dictionary<string, string>
                    {
                        { "Key", storeFixture.SendGridKey },
                        { "User", storeFixture.SendGridUser },
                    },
                },
                TemplateStorage = storeName,
                DefaultSender = new Internal.EmailAddress
                {
                    DisplayName = "test user",
                    Email = "no-reply@test.geeklearning.io"
                },
                Mockup = new MockupOptions
                {
                    Disclaimer = "",
                    Exceptions = new MockupExceptionsOptions
                    {
                        Domains = new List<string>(),
                        Emails = new List<string>(),
                    },
                    Recipients = new List<string>()
                }
            });

            var providerTypes = new List<IEmailProviderType>
            {
                new SendGrid.SendGridEmailProviderType(),
            };

            IEmailAddress address = new Internal.EmailAddress() {
                    DisplayName = "test user",
                    Email = "no-reply@test.geeklearning.io"
            };

            await emailSender.SendTemplatedEmailAsync("Notification1", new { }, address);
        }
    }
}
