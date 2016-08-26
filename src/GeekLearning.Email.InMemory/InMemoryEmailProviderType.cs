namespace GeekLearning.Email.InMemory
{
    public class InMemoryEmailProviderType : IEmailProviderType
    {
        public string Name => "InMemory";

        private IInMemoryEmailRepository inMemoryEmailRepository;

        public InMemoryEmailProviderType(IInMemoryEmailRepository inMemoryEmailRepository)
        {
            this.inMemoryEmailRepository = inMemoryEmailRepository;
        }

        public IEmailProvider BuildProvider(IEmailProviderOptions options)
        {
            return new InMemoryEmailProvider(options, this.inMemoryEmailRepository);
        }
    }
}
