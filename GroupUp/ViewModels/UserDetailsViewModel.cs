using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupUp.Models;

namespace GroupUp.ViewModels
{
    public class UserDetailsViewModel
    {
        public bool UserCanView { get; set; }

        public string DenyMessage { get; set; }

        public User User { get; set; }
    }
}