using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GroupUp.Models
{
    public class GroupReport
    {
        public int GroupReportId { get; set; }

        [Required] public Group TargetGroup { get; set; }

        [Required] public string Reason { get; set; }

        [Required] public string DetailedDescription { get; set; }


    }
}