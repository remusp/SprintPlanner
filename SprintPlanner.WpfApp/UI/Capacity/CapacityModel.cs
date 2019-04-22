using System.Collections.ObjectModel;

namespace SprintPlanner.WpfApp.UI.Capacity
{
    public class CapacityModel
    {
        public int DaysInSprint { get; set; }

        public ObservableCollection<UserDetails> Users { get; set; }
    }
}
