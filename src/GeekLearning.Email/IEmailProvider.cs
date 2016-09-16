namespace GeekLearning.Email
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEmailProvider
    {
        Task SendEmailAsync(IEmailAddress from, IEnumerable<IEmailAddress> recipients, string subject, string bodyText, string bodyHtml);
    }
}

