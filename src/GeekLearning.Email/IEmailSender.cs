namespace GeekLearning.Email
{
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task SendEmailAsync(string subject, string message, IEnumerable<IEmailAddress> recipients, AttachmentCollection attachments=null);

        Task SendEmailAsync(IEmailAddress from, string subject, string message, IEnumerable<IEmailAddress> recipients, AttachmentCollection attachments=null);

        Task SendTemplatedEmailAsync<T>(string templateKey, T context, IEnumerable<IEmailAddress> recipients, AttachmentCollection attachments=null);

        Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, IEnumerable<IEmailAddress> recipients, AttachmentCollection attachments=null);
    }
}
