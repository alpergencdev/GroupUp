using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupUp.Models;

namespace GroupUp.ViewModels
{
    public class GroupReportViewModel
    {
        public GroupReport GroupReport { get; set; }

        public int ReportedGroupId { get; set; }
    }
}