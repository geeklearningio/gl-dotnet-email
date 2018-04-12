using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekLearning.Email.SendGrid
{
    internal static class SendGridEmailHelpers
    {
        internal EmailAddress ToSendGridEmail(this IEmailAddress email)
        {
            return new EmailAddress(email.Email, email.DisplayName);
        }
    }
}
