using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace GroupUp.ViewModels
{
    public class GroupReportViewModel
    {
        [Required]
        public string Reason { get; set; }

        [Required]
        [Display(Name = "Detailed Description")]
        public string Description { get; set; }


        [Required]
        public string TargetGroupTitle { get; set; }

        public int ReportedGroupId { get; set; }
    }
}