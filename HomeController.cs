using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GeekLearning.Email.Internal;

namespace GeekLearning.Email.Samples.Controllers
{
    public class HomeController : Controller
    {
        private IEmailSender emailSender;

        public HomeController(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Usage:
        public async Task<IActionResult> SendEmail()
        {
            // Email addresses have Email, DisplayName, and an enum: AddressTarget (To,Cc,Bcc)
            List<IEmailAddress> recipients = new List<IEmailAddress>() {
                new EmailAddress() { Email = "myfriend@aways.com", DisplayName = "Samuel", AddressAs = AddressTarget.To },
                new EmailAddress() { Email = "rhsmith@gworld.com", DisplayName = "Bob", AddressAs = AddressTarget.Cc },
                new EmailAddress() { Email = "igetit@world.gov", DisplayName="George Jones", AddressAs= AddressTarget.ReplyTo }
            };


            // example of how to add a simple attachment. Add images, streams, etc as byte arrays, for example:

            MimeKit.AttachmentCollection attachments = new MimeKit.AttachmentCollection
            {
                { "sample_attachment.txt", System.Text.Encoding.UTF8.GetBytes("This is the content of the file attachment.") }
            };

            // the Reply-To addresses are simply another list of IEmailAddress objects, here, were are ignoring them as null.
            // Also, in the From address, the AddressAs property is ignored, and the From address is positional and always treated as the From.
            // Likewise, a From enumeration value in the recipients list is ignored, and will be treated as a To address.
            await this.emailSender.SendEmailAsync(new EmailAddress() { Email="to.somebody@domain.tld", DisplayName="Me", AddressAs=AddressTarget.From }, "A simple message","This is a test message", recipients, attachments);


            // Here is a second send example. No attachments, but using templates:

            recipients.Clear();
            recipients.Add(new EmailAddress { Email="george@alaska.edu", DisplayName="George Jones", AddressAs=AddressTarget.To });
            var context = new
            {
                ApplicationName = "Email Sender Sample",
                User = recipients
            };
            await this.emailSender.SendTemplatedEmailAsync("Invitation", context, recipients );
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
