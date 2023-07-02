using SprintPlanner.Core.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprintPlanner.Core.BusinessModel
{
    public class Team
    {
        public Guid ServerId { get; set; }

        public List<UserDetailsModel> Users { get; set; }
    }
}
