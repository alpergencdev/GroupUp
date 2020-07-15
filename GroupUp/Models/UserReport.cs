using System.ComponentModel.DataAnnotations;
namespace GroupUp.Models
{
    public class UserReport
    {
        public int UserReportId { get; set; }

        [Required]
        public User TargetUser { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public string DetailedDescription { get; set; }


    }
}