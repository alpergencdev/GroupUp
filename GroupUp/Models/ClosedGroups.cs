using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GroupUp.Models
{
    public class ClosedGroup
    {
        public int ClosedGroupId { get; set; }
        [Required]
        public Group Group { get; set; }

        public IList<User> RatedUsers { get; set; }

    }
}