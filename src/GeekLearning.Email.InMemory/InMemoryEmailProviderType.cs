namespace GeekLearning.Email.InMemory
{
    public class InMemoryEmailProviderType : IEmailProviderType
    {
        public string Name => "InMemory";

        public IEmailProvider BuildProvider(IEmailProviderOptions options)
        {
            return new InMemoryEmailProvider(options);
        }
    }
}
