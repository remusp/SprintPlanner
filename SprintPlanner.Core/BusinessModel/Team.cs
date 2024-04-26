using SprintPlanner.Core.Logic;
using System;
using System.Collections.Generic;

namespace SprintPlanner.Core.BusinessModel
{
    public class Team
    {
        public Guid ServerId { get; set; }

        public List<UserDetailsModel> Users { get; set; }
    }
}
