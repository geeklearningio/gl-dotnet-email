namespace GeekLearning.Email.Integration.Test
{
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;

    public class Datas
    {
        public const string FirstRecipient = "no-reply@test.geeklearning.io";
        public const string SecondRecipient = "no-reply2@test.geeklearning.io";
        public const string ThirdRecipient = "no-reply3@test.geeklearning.io";

        public static IOptions<EmailOptions> GetOptions(StoresFixture storeFixture, string storeName = null)
        {
            return Options.Create(new EmailOptions
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
        }
    }
}
