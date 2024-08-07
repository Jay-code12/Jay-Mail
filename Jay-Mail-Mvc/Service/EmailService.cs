using Jay_Mail_Mvc.Dto;
using Jay_Mail_Mvc.IRepository;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Net.Mail;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Jay_Mail_Mvc.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettingsDto _emailSettings;
        public EmailService(IOptions<EmailSettingsDto> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendEmailAsync(EmailDto emailDto, IFormFile file)
        {
            var email = CreateEmail(emailDto, file);
            await SendMessage(email, emailDto);
            return true;

        }

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
    }
}
