using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupUp.Models;

namespace GroupUp.ViewModels
{
    public class GroupRequestsViewModel
    {
        public IList<Group> Groups { get; set; }
        public User User { get; set; }
        public LocationProperties Location { get; set; }
    }
}