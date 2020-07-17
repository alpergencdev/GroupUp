using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupUp.Models;

namespace GroupUp.ViewModels
{
    public class ModeratorUserReportsViewModel
    {
        public IList<UserReport> UserReports { get; set; }
    }
}