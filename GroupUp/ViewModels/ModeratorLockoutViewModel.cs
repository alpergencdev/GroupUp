using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GroupUp.ViewModels
{
    public class ModeratorLockoutViewModel
    {
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name="Enter new lockout date in the format given below.")]
        public DateTime LockoutDate { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}