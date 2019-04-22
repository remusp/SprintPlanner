using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class SprintModel
    {

        public ObservableCollection<KeyValuePair<int,string>> Boards { get; set; }

        public int SelectedBoard { get; set; }

        public ObservableCollection<KeyValuePair<int,string>> Sprints { get; set; }

        public int SelectedSprint { get; set; }
    }
}
