using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GroupUp.Models;

namespace GroupUp.ViewModels
{
    public class UserEditViewModel
    {
        [Required]
        public string PreviousEmail { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name="E-mail Address")]
        [IsUniqueEmail]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Contact Information:")]
        public string ContactInfo { get; set; }
    }

    public class IsUniqueEmail : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var thisViewModel = (UserEditViewModel) validationContext.ObjectInstance;

            if (thisViewModel.Email != thisViewModel.PreviousEmail && context.Users.Any(u => u.AspNetIdentity.Email == thisViewModel.Email))
            {
                return new ValidationResult("The e-mail address you have requested is already in use.");
            }

            return ValidationResult.Success;
        }
    }
}