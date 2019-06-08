using System;
using System.Collections.Generic;
using System.Text;

namespace SprintPlanner.Core.Logic
{
    public class DataStorageModel
    {
        public SprintStorageModel Sprint { get; set; }

        public CapacityStorageModel Capacity { get; set; }
    }
}
