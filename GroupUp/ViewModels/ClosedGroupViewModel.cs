using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupUp.Models;

namespace GroupUp.ViewModels
{
    public class ClosedGroupViewModel
    {
        public ClosedGroup ClosedGroup { get; set; }

        public bool UserAlreadyRated { get; set; }

        public User User { get; set; }
    }
}