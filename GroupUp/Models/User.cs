using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GroupUp.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string ContactInfo { get; set; }

        [Required]
        public int SecurityLevel { get; set; }

        [Required]
        public int TrustPoints { get; set; }

        [Required]
        public ApplicationUser AspNetIdentity { get; set; }

        [Required] 
        public IList<Group> Groups { get; set; }

        public IList<ClosedGroup> ClosedGroups { get; set; }
        public int? VerificationCode { get; set; }

        public bool IsVerified { get; set; }

        public bool IsModerator { get; set; }

    }
}