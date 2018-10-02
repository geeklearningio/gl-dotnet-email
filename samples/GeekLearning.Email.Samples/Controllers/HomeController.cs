using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekLearning.Email.Internal;
using Microsoft.AspNetCore.Mvc;

namespace GeekLearning.Email.Samples.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailSender emailSender;

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

            await this.emailSender.SendTemplatedEmailAsync("Invitation", context, user);

            return RedirectToAction("Index");
        }

        [Route("send-cc")]
        public async Task<IActionResult> SendEmailWithCc()
        {
            var user = new User
            {
                Email = "john@doe.me",
                DisplayName = "John Doe"
            };

            var userCc = new User
            {
                Email = "contensa@contenso.me",
                DisplayName = "Maria Contensa"
            };

            var context = new
            {
                ApplicationName = "Email Sender Sample",
                User = user
            };

            var sender = new EmailAddress("defaultsender@doe.me", "Sender");
            await this.emailSender.SendTemplatedEmailAsync(sender, "Invitation", context, Enumerable.Empty<IEmailAttachment>(), new IEmailAddress[] { user }, new IEmailAddress[] { userCc}, new IEmailAddress[0] );

            return RedirectToAction("Index");
        }

        [Route("send-bcc")]
        public async Task<IActionResult> SendEmailWithBcc()
        {
            var user = new User
            {
                Email = "john@doe.me",
                DisplayName = "John Doe"
            };

            var userBcc = new User
            {
                Email = "contensa@contenso.me",
                DisplayName = "Maria Contensa"
            };

            var context = new
            {
                ApplicationName = "Email Sender Sample",
                User = user
            };

            var sender = new EmailAddress("defaultsender@doe.me", "Sender");
            await this.emailSender.SendTemplatedEmailAsync(sender, "Invitation", context, Enumerable.Empty<IEmailAttachment>(), new IEmailAddress[] { user },new IEmailAddress[0], new IEmailAddress[] { userBcc });

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

        [Route("send-attachments")]
        public async Task<IActionResult> SendEmailAttachments()
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

            var data = System.IO.File.ReadAllBytes(@"Files\beach.jpeg");
            var image = new EmailAttachment("Beach.jpeg", data, "image", "jpeg");

            data = System.IO.File.ReadAllBytes(@"Files\sample.pdf");
            var pdf = new EmailAttachment("Sample.pdf", data, "application", "pdf");

            await this.emailSender.SendTemplatedEmailAsync(
                new EmailAddress("defaultsender@doe.me", "Sender"), 
                "Invitation", 
                context, 
                new List<IEmailAttachment> { image, pdf }, 
                user);

            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
