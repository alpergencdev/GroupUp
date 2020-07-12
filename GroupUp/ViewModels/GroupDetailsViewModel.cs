using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupUp.Models;

namespace GroupUp.ViewModels
{
    public class GroupDetailsViewModel
    {
        public Group Group { get; set; }

        public User User { get; set; }
    }
}