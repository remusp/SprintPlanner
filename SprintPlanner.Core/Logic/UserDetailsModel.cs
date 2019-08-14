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

        public decimal Capacity { get { return (DaysInSprint - DaysOff) * HoursPerDay; } }

        public decimal ScaledCapacity { get { return Capacity * CapacityFactor; } }
    }
}
