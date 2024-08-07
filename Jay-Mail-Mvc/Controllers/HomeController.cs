using Jay_Mail_Mvc.Dto;
using Jay_Mail_Mvc.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Jay_Mail_Mvc.Controllers
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
        public async Task<IActionResult> Index(EmailDto emailDto, IFormFile file)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            emailDto.Body = GetHtmlContent(emailDto.Body);

            if (await _emailService.SendEmailAsync(emailDto, file))
            {
                TempData["message"] = true;
            }
            else
            {
                TempData["message"] = false;
            }

            return RedirectToAction("Index");
        }

        private string GetHtmlContent(string message)
        {
            DateTime date = DateTime.Today;

            int year = date.Year;

            //string response = $"<h1>Hello Buddy</h1> {message} <h1>SuccessFul</h1>";
            string response = "<!DOCTYPE html> <html lang='en'><head> </head> </body>";

            response += "  <link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css\" rel=\"stylesheet\">\r\n";

            response += "<div style='background: #f5f5f5 padding: 20px'>Jay-Mail</div>";
            response += "<h3 style='text-align:center'>checking</h3>";
            response += $"<p class='text-success mb-2'>{message}</p>";
            response += "<p style='color:red'>Thumbs Up</p>";
            response += "<p>leo. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Nullam id dolor id nibh ultricies vehicula.</p>";
            response += "<p>lorem ipsum lorem ipsum lorem ipsum</p>";
            response += $"<small class='bg-light p-5 mt-4 text-center'>&copy; copyright {year} J-code developer</small>";

            response += "<script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js\"></script>";
            response += "</body></html>";

            return response;
        }


    }
}
