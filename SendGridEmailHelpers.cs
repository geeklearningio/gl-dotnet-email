namespace GeekLearning.Email.SendGrid
{
    using global::SendGrid.Helpers.Mail;
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal static class SendGridEmailHelpers
    {
        internal static EmailAddress ToSendGridEmail(this IEmailAddress email)
        {
            return new EmailAddress(email.Email, email.DisplayName);
        }
    }
}
