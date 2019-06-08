using System;
using System.Collections.Generic;
using System.Text;

namespace SprintPlanner.Core.Logic
{
    public class UserDetailsModel
    {
        public string Uid { get; set; }

        public string UserName { get; set; }

        public decimal HoursPerDay { get; set; }

        public int DaysOff { get; set; }

        public int DaysInSprint { get; set; }

        public decimal CapacityFactor { get; set; }
    }
}
