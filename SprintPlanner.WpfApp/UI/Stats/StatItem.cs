using SprintPlanner.FrameworkWPF;

namespace SprintPlanner.WpfApp.UI.Stats
{
    internal class StatItem : ViewModelBase
    {
        public string StatName { get; set; }

        public decimal PlannedCapacity { get; set; }

        public decimal FullCapacity { get; set; }

        public decimal TresholdCapacity { get; set; }
    }
}
