namespace GeekLearning.Email
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task SendEmailAsync(string subject, string message, params IEmailAddress[] to);

        Task SendEmailAsync(IEmailAddress from, string subject, string message, params IEmailAddress[] to);

        Task SendEmailAsync(IEmailAddress from, IEmailAddress replyTo, string subject, string message, bool plainTextOnly, params IEmailAddress[] to);

        Task SendEmailAsync(IEmailAddress from, string subject, string message, IEnumerable<IEmailAttachment> attachments, params IEmailAddress[] to);

        Task SendEmailAsync(IEmailAddress from, IEmailAddress replyTo, string subject, string message, bool plainTextOnly, IEnumerable<IEmailAttachment> attachments, params IEmailAddress[] to);

        Task SendEmailAsync(IEmailAddress from, string subject, string message, IEnumerable<IEmailAttachment> attachments, IEmailAddress[] to, IEmailAddress[] cc, IEmailAddress[] bcc, IEmailAddress replyTo = null, bool plainTextOnly = false);

        Task SendTemplatedEmailAsync<T>(string templateKey, T context, params IEmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, params IEmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(IEmailAddress from, IEmailAddress replyTo, string templateKey, T context, params IEmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, IEnumerable<IEmailAttachment> attachments, params IEmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(IEmailAddress from, IEmailAddress replyTo, string templateKey, T context, IEnumerable<IEmailAttachment> attachments, params IEmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(IEmailAddress from, string templateKey, T context, IEnumerable<IEmailAttachment> attachments, IEmailAddress[] to, IEmailAddress[] cc, IEmailAddress[] bcc, IEmailAddress replyTo = null);
    }
}
