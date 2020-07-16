using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GroupUp.ViewModels
{
    public class UserEditViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name="E-mail Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Contact Information:")]
        public string ContactInfo { get; set; }
    }
}