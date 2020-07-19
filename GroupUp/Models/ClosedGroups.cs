using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GroupUp.Models
{
    /// <summary> 

    ///  This class is an entry for closed groups, to keep track of which users have rated their group members.

    /// </summary> 
    public class ClosedGroup
    {
        public int ClosedGroupId { get; set; }
        [Required]
        public Group Group { get; set; }

        public IList<User> RatedUsers { get; set; }

    }
}