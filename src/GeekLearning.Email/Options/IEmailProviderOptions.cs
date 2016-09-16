namespace GeekLearning.Email
{
    using System.Collections.Generic;

    public interface IEmailProviderOptions
    {
        string Type { get; set; }

        Dictionary<string, string> Parameters { get; set; }
    }
}
