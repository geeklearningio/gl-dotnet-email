namespace GeekLearning.Email
{
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task SendEmailAsync(string subject, string message, AttachmentCollection attachments, params IEmailAddress[] to);
        
        Task SendEmailAsync(string subject, string message, params IEmailAddress[] to);

        Task SendEmailAsync(IEmailAddress from, string subject, string message, params IEmailAddress[] to);

        Task SendEmailAsync(IEmailAddress from, string subject, string message, AttachmentCollection attachments, params IEmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(string templateKey, T context, AttachmentCollection attachments, params IEmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(string templateKey, T context, params IEmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, params IEmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, AttachmentCollection attachments, params IEmailAddress[] to);
    }
}
