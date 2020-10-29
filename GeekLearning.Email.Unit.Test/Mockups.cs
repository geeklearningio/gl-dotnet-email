namespace GeekLearning.Email.Unit.Test
{
    using GeekLearning.Email.Internal;
    using GeekLearning.Storage;
    using GeekLearning.Templating;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class Mockups
    {
        [Fact]
        public void TestMockupRecipients()
        {
            var emailProvider = new Mock<IEmailProvider>();
            var emailProviderTypes = new List<IEmailProviderType> { new TestProviderType(emailProvider.Object) };

            var emailSender = new EmailSender(emailProviderTypes, Datas.GetMockupOptions(), new Mock<IStorageFactory>().Object, new Mock<ITemplateLoaderFactory>().Object);
            emailSender.SendEmailAsync("test", "test", new EmailAddress("test@test.fr", "test"));

            emailProvider.Verify(e => e.SendEmailAsync(
                Datas.DefaultSender,
                It.Is<IEnumerable<IEmailAddress>>(list => list.Any(a => a.DisplayName == "Mockup Recipient" && a.Email == Datas.MockedUpRecipient)),
                It.IsAny<IEnumerable<IEmailAddress>>(),
                It.IsAny<IEnumerable<IEmailAddress>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                Enumerable.Empty<IEmailAttachment>(),
                null), Times.Once);
        }

        [Fact]
        public void TestMockupRecipientsCc()
        {
            var emailProvider = new Mock<IEmailProvider>();
            var emailProviderTypes = new List<IEmailProviderType> { new TestProviderType(emailProvider.Object) };

            var emailSender = new EmailSender(emailProviderTypes, Datas.GetMockupOptions(), new Mock<IStorageFactory>().Object, new Mock<ITemplateLoaderFactory>().Object);
            emailSender.SendEmailAsync(Datas.DefaultSender, "test", "test", Enumerable.Empty<IEmailAttachment>(), new EmailAddress("test@test.fr", "test").Yield(), new EmailAddress("test@test.fr", "test").Yield(), new EmailAddress[0]);

            emailProvider.Verify(e => e.SendEmailAsync(
                Datas.DefaultSender,
                It.Is<IEnumerable<IEmailAddress>>(list => list.All(a => a.DisplayName == "Mockup Recipient" && a.Email == Datas.MockedUpRecipient)),
                 It.Is<IEnumerable<IEmailAddress>>(list => list.All(a => a.DisplayName == "Mockup Recipient" && a.Email == Datas.MockedUpRecipient)),
                It.IsAny<IEnumerable<IEmailAddress>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                Enumerable.Empty<IEmailAttachment>(),
                null), Times.Once);
        }
    }

    public class TestProviderType : IEmailProviderType
    {
        public string Name => "Fake";

        public IEmailProvider EmailProvider { get; }

        public TestProviderType(IEmailProvider emailProvider)
        {
            EmailProvider = emailProvider;
        }

        public IEmailProvider BuildProvider(IEmailProviderOptions providerOptions)
        {
            return EmailProvider;
        }
    }
}
