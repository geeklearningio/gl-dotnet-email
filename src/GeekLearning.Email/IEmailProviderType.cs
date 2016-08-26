namespace GeekLearning.Email
{
    public interface IEmailProviderType
    {
        string Name { get; }

        IEmailProvider BuildProvider(IEmailProviderOptions providerOptions);
    }
}