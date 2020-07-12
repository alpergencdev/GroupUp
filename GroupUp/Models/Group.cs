using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace GroupUp.Models
{
    public class Group
    {
        public int GroupId { get; set; }

        [Required] public string Title { get; set; }

        [Required] public string Description { get; set; }

        [Required] public User Creator { get; set; }

        [Required] public int MaxUserCapacity { get; set; }

        [Required] public string City { get; set; }

        [Required] public string Country { get; set; }

        [Required] public string Continent { get; set; }

        [Required] public IList<User> Members { get; set; }
    }
}
