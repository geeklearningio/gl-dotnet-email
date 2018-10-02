namespace GeekLearning.Email.Unit.Test
{
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    public class Datas
    {
        public const string FirstRecipient = "no-reply@test.geeklearning.io";
        public const string SecondRecipient = "no-reply2@test.geeklearning.io";
        public const string ThirdRecipient = "no-reply3@test.geeklearning.io";
        public static Internal.EmailAddress DefaultSender = new Internal.EmailAddress
        {
            DisplayName = "test user",
            Email = "no-reply@test.geeklearning.io"
        };
        public const string MockedUpRecipient = "mockedup@test.geeklearning.io";
        public static IOptions<EmailOptions> GetMockupOptions()
        {
            return Microsoft.Extensions.Options.Options.Create(new EmailOptions
            {
                Provider = new EmailProviderOptions
                {
                    Type = "Fake",
                },
                DefaultSender = DefaultSender,
                Mockup = new MockupOptions
                {
                    Disclaimer = "Warning, recipients are mocked.",
                    Exceptions = new MockupExceptionsOptions
                    {
                        Domains = new List<string>(),
                        Emails = new List<string>(),
                    },
                    Recipients = new List<string> { MockedUpRecipient }
                }
            });
        }
    }
}
