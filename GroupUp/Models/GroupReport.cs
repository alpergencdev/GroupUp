using System.ComponentModel.DataAnnotations;

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