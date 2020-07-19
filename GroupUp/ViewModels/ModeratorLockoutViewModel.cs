using System;
using System.ComponentModel.DataAnnotations;

namespace GroupUp.ViewModels
{
    public class ModeratorLockoutViewModel
    {
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name="Enter new lockout date in the format given below.\n Enter 1.01.2099 00:00:00 if you want the user to be blocked indefinitely.")]
        public DateTime LockoutDate { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}