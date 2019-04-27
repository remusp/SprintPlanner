using System;
using System.Collections.ObjectModel;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class SprintModel
    {

        public ObservableCollection<Tuple<int,string>> Boards { get; set; }

        public int SelectedBoard { get; set; }

        public ObservableCollection<Tuple<int,string>> Sprints { get; set; }

        public int SelectedSprint { get; set; }
    }
}
