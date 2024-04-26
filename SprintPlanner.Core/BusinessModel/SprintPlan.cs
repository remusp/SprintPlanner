using System;

namespace SprintPlanner.Core.BusinessModel
{
    public class SprintPlan
    {
        public string PlanName { get; set; }

        public Sprint Sprint { get; set; }

        public Guid ServerId { get; set; }
        
        public Team TeamAvailability { get; set; }
    }
}
