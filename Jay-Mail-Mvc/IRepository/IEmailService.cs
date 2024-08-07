using Jay_Mail_Mvc.Dto;

namespace Jay_Mail_Mvc.IRepository
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailDto emailDto, IFormFile file);

    }
}
