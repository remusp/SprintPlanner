using System;
using System.Collections.Generic;

namespace SprintPlanner.Core.Logic
{
    public class SprintStorageModel
    {

        public List<Tuple<int, string>> Boards { get; set; }

        public int SelectedBoard { get; set; }

        public List<Tuple<int, string>> Sprints { get; set; }

        public int SelectedSprint { get; set; }
    }
}
