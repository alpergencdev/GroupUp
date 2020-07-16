using System.ComponentModel.DataAnnotations;
using GroupUp.Models;

namespace GroupUp.ViewModels
{
    public class UserReportViewModel
    {
        [Required]
        public string Reason { get; set; }

        [Required]
        [Display(Name="Detailed Description")]
        public string Description { get; set; }

        [Required]
        public string TargetUsername { get; set; }

        [Required]
        public int ReportedUserId { get; set; }
    }
}