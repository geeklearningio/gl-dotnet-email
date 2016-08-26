namespace GeekLearning.Email.SendGrid
{
    public class SendGridEmailProviderType : IEmailProviderType
    {
        public string Name => "SendGrid";

        public IEmailProvider BuildProvider(IEmailProviderOptions options)
        {
            return new SendGridEmailProvider(options);
        }
    }
}
