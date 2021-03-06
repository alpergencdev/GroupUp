﻿using System.Collections.Generic;
using GroupUp.Models;

namespace GroupUp.ViewModels
{
    public class UserGroupsViewModel
    {
        public List<Group> JoinedGroups { get; set; }
        public List<Group> CreatedGroups { get; set; }

        public List<Group> ClosedGroups { get; set; }
        public User User { get; set; }
    }
}