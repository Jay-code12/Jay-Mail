using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Jay_Mail_Mvc.Dto
{
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
}
