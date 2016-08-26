namespace GeekLearning.Email.Smtp
{
    public class SmtpEmailProviderType : IEmailProviderType
    {
        public string Name => "Smtp";

        public IEmailProvider BuildProvider(IEmailProviderOptions options)
        {
            return new SmtpEmailProvider(options);
        }
    }
}
