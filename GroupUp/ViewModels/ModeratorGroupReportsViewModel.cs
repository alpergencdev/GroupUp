using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupUp.Models;

namespace GroupUp.ViewModels
{
    public class ModeratorGroupReportsViewModel
    {
        public IList<GroupReport> GroupReports { get; set; }
    }
}