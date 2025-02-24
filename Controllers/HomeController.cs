using EmailSendingApp.Models;
using EmailSendingApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmailSendingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailService _emailService;

        public HomeController(IEmailService emailService)
        {
            _emailService = emailService;
        }  

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                await _emailService.SendEmailAsync(model.ToEmail, model.Subject, model.Body);
                ViewData["Message"] = "Email sent successfully!";
            }

            return View("Index");
        }
    }
}
