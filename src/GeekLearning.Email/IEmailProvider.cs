namespace GeekLearning.Email
{

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MimeKit;

    public interface IEmailProvider
    {
        Task SendEmailAsync(
            IEmailAddress from, 
            IEnumerable<IEmailAddress> recipients, 
            string subject, 
            string bodyText, 
            string bodyHtml, 
            AttachmentCollection attachments=null);
    }
}
