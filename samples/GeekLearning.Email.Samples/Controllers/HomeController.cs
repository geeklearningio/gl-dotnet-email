using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> SendEmail()
        {
 
            EmailAddress toAddress1 = new EmailAddress() { Email = "rhsmith@gworld.com", DisplayName = "Bob" };
            EmailAddress toAddress2 = new EmailAddress() { Email = "sammy.davis@null.com", DisplayName = "Sam" };

            // example of how to add a simple attachment. Add images, streams, etc as byte arrays, for example:

            MimeKit.AttachmentCollection attachments = new MimeKit.AttachmentCollection
            {
                { "sample_attachment.txt", System.Text.Encoding.UTF8.GetBytes("This is the content of the file attachment.") }
            };



            await this.emailSender.SendEmailAsync(new EmailAddress() { Email="from.somebody@domain.tld", DisplayName="Me" }, "A simple message","This is a test message", attachments, toAddress1);


            // Here is a second send example. No attachments, but using templates. Specifies to send a Cc to ccRecipient, using a decorator:

            IEmailAddress ccRecipient = new EmailAddress() { Email = "myfriend@somewhere.com", DisplayName = "Joe Smith" };

            var context = new
            {
                ApplicationName = "Email Sender Sample",
                User = toAddress1
            };
            await this.emailSender.SendTemplatedEmailAsync("Invitation", context, toAddress2, ccRecipient.ToCc()  );


            return RedirectToAction("Index");
        }

        [Route("send-chars")]
        public async Task<IActionResult> SendSpecialCharactersEmail()
        {
            var user = new User
            {
                Email = "john@doe.me",
                DisplayName = "John Doe"
            };

            var context = new
            {
                ApplicationName = "Email Sender Sample",
                User = user
            };

            await this.emailSender.SendTemplatedEmailAsync("SpecialChar", context, user);

            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
