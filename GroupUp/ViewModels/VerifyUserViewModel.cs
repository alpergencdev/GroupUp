using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GroupUp.ViewModels
{
    public class VerifyUserViewModel
    {
        [Required]
        [Range(100000, 999999)]
        public int VerificationCode { get; set; }
    }
}