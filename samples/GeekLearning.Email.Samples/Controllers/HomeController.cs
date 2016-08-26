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

        public IActionResult Error()
        {
            return View();
        }
    }
}
