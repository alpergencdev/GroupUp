using System.ComponentModel.DataAnnotations;

namespace GroupUp.ViewModels
{
    public class VerifyUserViewModel
    {
        [Required]
        [Range(100000, 999999)]
        public int VerificationCode { get; set; }
    }
}