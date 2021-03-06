﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GroupUp.Models
{
    public class Group
    {
        public int GroupId { get; set; }

        [Required] public string Title { get; set; }

        [Required] public string Description { get; set; }

        [Required] public User Creator { get; set; }

        [Range(2, 50)]
        [Required] public int MaxUserCapacity { get; set; }

        [Required] public string City { get; set; }

        [Required] public string Country { get; set; }

        [Required] public string Continent { get; set; }

        [Required] public IList<User> Members { get; set; }

        public string ChatLog { get; set; }

        public bool IsClosed { get; set; }
    }
}
