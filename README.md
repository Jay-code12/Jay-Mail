**### Simplifying Multi-Recipient Email Sending**

My objectives are

- reach multiple target at a time with customized _email_template
- Implement robust email templating for professional-looking messages
- enabling sending of attachments along email if necessary 

> <img width="956" alt="home" src="https://github.com/user-attachments/assets/9e7550cd-bab8-4ca1-b61b-688c9226b7d2">

> <img width="654" alt="tbd" src="https://github.com/user-attachments/assets/26ed27bb-f28d-4aac-8637-ff543b03639e">

> <img width="148" alt="phoe" src="https://github.com/user-attachments/assets/1dc39b98-b809-494d-b76a-aaf997782b39">


**Create asp.net core Mvc project _J-Mail_**

- create EmailService C# file
- create IEmailService C# file
- create a Dto file name EmailDto
- create a model file name EmailSettings
- create a controller _SendEmailController_

> **EmailDto**` 
this file contails model property for our view file, it will help us collect and store all input data from the user code below

```
public class EmailDto
    {
        public string Email { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Subject must be between 3 and 50 characters")]
        [Required]
        public string Subject { get; set; }

        [StringLength(200, MinimumLength = 5, ErrorMessage = "Message must be between 5 and 200 characters")]
        [Required]
        public string Body { get; set; }

        [ValidateNever]
        public string FilePath { get; set; }

    }

```

> **EmailSettings**` 
this model file will help us collect and store the value of our emailconfiguration json from the appsettings files, will we confige it on the program.cs file to get values from the appsettings json file

```
    public class EmailSettingsDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

```
> **EmailService **` 
this file will contains all our code we will be needing to perform our sending email action action, we will be having three method on this class with a _constructor_ for injection the _EmailSettings_ file so we can access our email configuration file from appsetting
```
        private readonly EmailSettingsDto _emailSettings;
        public EmailService(IOptions<EmailSettingsDto> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
```
- the sendEmail function that will be called on the SendEmailController to trigger action and return true if successful

```
        public async Task<bool> SendEmailAsync(EmailDto emailDto, IFormFile file)
        {
            var email = CreateEmail(emailDto, file);
            await SendMessage(email, emailDto);
            return true;

        }
```

- the create Email message usign MineMessage

```
        private MimeMessage CreateEmail(EmailDto emailDto, IFormFile file)
        {
            var email = new MimeMessage();

            email.Sender = MailboxAddress.Parse(_emailSettings.Email);
            email.From.Add(new MailboxAddress("J-Mail", "2bP8A@j-mail.com"));
            email.ReplyTo.Add(new MailboxAddress("J-Mail", "eze7011@gmail.com"));
            email.Subject = emailDto.Subject;

            //email.To.Add(MailboxAddress.Parse(emailDto.Email));
            //email.To.AddRange(emailDto.Email);

            MemoryStream memoryStream = new MemoryStream();
            var builder = new BodyBuilder();
            builder.HtmlBody = emailDto.Body;

            if (file != null)
            {
                using (var stream = file.OpenReadStream())
                {
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, (int)stream.Length);
                    builder.Attachments.Add(file.FileName, buffer);
                }
            }

            email.Body = builder.ToMessageBody();
            email.Body = new TextPart(TextFormat.Html) { Text = emailDto.Body };


            return email;
        } 
```

-  the SendMessage class which will be responsible for sending the email using SmtpClient

```
         private async Task SendMessage(MimeMessage email, EmailDto emailDto)
        {
            using var smtp = new SmtpClient();

            smtp.Connect(_emailSettings.Host, _emailSettings.Port, true);
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);

            string str = emailDto.Email;
            string[] array = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < array.Length; i++)
            {
                //email.Bcc.Add(new MailboxAddress(null, recipient));
                email.To.Add(MailboxAddress.Parse(array[i]));
                await smtp.SendAsync(email);
                email.To.Clear();
            }

            smtp.Disconnect(true);
        }

```

> **IEmailService **` 
this class will be use as the dendency injection to call on the email SendMail method for clean code and easy feature mentainance 

code should be added to our  program.cs file
`builder.Services.AddTransient<IEmailService, EmailService>();
`
```
    public interface IEmailService
    {
        Task<bool> SendEmail> **IEmailService **` 

    }
```

> **SendEmailController**` 
here we will be calling on the IEmailService dependency to send email if user input is acceptable

```
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
```

**### our view file** 

```
@model Jay_Mail_Mvc.Dto.EmailDto

@{
    ViewData["Title"] = "Home Page";
}


<section>

    <div class="row bg-white border-rounded" id="form-container" row>

        <form class="m-auto" asp-action="Index" method="post"  enctype="multipart/form-data">
            <ul class="border border-primary">
                <strong>Note</strong>
                <li class="text-warning">
                    <small>send same message to multiple email address</small>
                </li>
                <li class="text-info">
                    <small>seperate each emails with empty space e.g (gmail.com gmail.com)</small>
                </li>
                <li class="text-secondary ">
                    <small>send mail To your customers today with beautiful made template </small>
                </li>
            </ul>

            @if (((bool?)TempData["message"]) == true)
            {
                <div class="alert alert-dismissible alert-success">
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    <strong>Congratulation!</strong> sent
                </div>
            }
            else if (((bool?)TempData["message"]) == false)
            {
                <div class="alert alert-dismissible alert-danger">
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    <strong>Error!</strong> not sent
                </div>
            }
            else{}

            <div asp-validation-summary="ModelOnly" class="text-danger">

            </div>

            <div >
                <div class="form-group">
                    <div class="input-group mb-">
                        <button class="btn btn-dark" type="button" id="button-addon2">
                            <i class="fa-solid fa-user">subject</i>
                        </button>
                        <input type="text" class="form-control" placeholder="E.g Jogn Doe" aria-label="Recipient's username" aria-describedby="button-addon2" asp-for="Subject" >
                    </div>
                    <span asp-validation-for="Subject" class="text-danger"></span>
                </div>
                <div class="form-group mt-3">
                    <div class="input-group mb-">
                        <button class="btn btn-dark" type="button" id="button-addon2">
                            <i class="fa-solid fa-user">Email</i>
                        </button>
                        <input type="text" class="form-control" placeholder="E.g Jogn Doe" aria-label="Recipient's username" aria-describedby="button-addon2" asp-for="Email">
                    </div>
                    <span asp-validation-for="Email" class="text-danger"></span>
                </div>
                <div class="mt-2">
                    <label for="exampleFormControlTextarea1" class="form-label">Message Box</label>
                    <textarea class="form-control" id="exampleFormControlTextarea1" rows="3" placeholder="Enter your message here" asp-for="Body" ></textarea>
                </div>
                <div class="form-group mt-3">
                    <div class="input-group mb-">
                        <button class="btn btn-dark" type="button" id="button-addon2">
                            <i class="fa-solid fa-user">File</i>
                        </button>
                        <input type="file" name="file" class="form-control" placeholder="E.g Jogn Doe" aria-label="upload file" aria-describedby="button-addon2" asp-for="FilePath">
                    </div>
                    <span asp-validation-for="FilePath" class="text-danger"></span>
                </div>
                <span asp-validation-for="Body" class="text-danger"></span>
                <div class="form-group mt-3">
                    <button type="submit" class="btn btn-dark py-2">Send</button>
                </div>
            </div>
        </form>

        <div class="m-auto">
            <p class="text-center">
                formal coding instructor and currently do freelancing job i involve in the full lifecycle development of your Web app my focus are:
            </p>
            <ul class="m-auto" style="width: 100%; justify-self: center;">
                <li>To demonstrating strong project management</li>
                <li>saving client extra cost at feature using api for backend development</li>
                <li>Strong Code architecture for easy future mentainance</li>
            </ul>

            <img align="right" alt="Coding" width="400" src="https://i.pinimg.com/550x/54/e3/7d/54e37d8074ebcde1d96c77d7b2a7f310.jpg" />

        </div>

    </div>
</section>
```
