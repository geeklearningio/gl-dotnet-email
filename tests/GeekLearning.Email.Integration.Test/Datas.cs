namespace GeekLearning.Email.Integration.Test
{
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    public class Datas
    {
        public const string FirstRecipient = "annayafi@gmail.com";
        public const string SecondRecipient = "anna_yafi@gmail.com";
        public const string ThirdRecipient = "nomorecries@gmail.com";
        public static IOptions<EmailOptions> GetOptions(StoresFixture storeFixture, string storeName = null)
        {
            return Microsoft.Extensions.Options.Options.Create(new EmailOptions
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
