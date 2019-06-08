using System;
using System.Collections.Generic;
using System.Text;

namespace SprintPlanner.Core.Logic
{
    public class CapacityStorageModel
    {
        public int DaysInSprint { get; set; }

        public List<UserDetailsModel> Users { get; set; }

        public decimal CapacityFactor { get; set; }
    }
}
