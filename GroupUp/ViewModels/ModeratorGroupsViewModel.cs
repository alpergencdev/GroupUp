using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupUp.Models;

namespace GroupUp.ViewModels
{
    public class ModeratorGroupsViewModel
    {
        public IList<Group> Groups { get; set; }
    }
}