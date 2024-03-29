﻿using SprintPlanner.Core.BusinessModel;
using System;
using System.Collections.Generic;

namespace SprintPlanner.Core.Logic
{
    public class SprintCrudStorageModel
    {
        public List<Tuple<int, string>> Boards { get; set; }

        public int SelectedBoard { get; set; }

        public List<BusinessModel.Sprint> Sprints { get; set; }

        public BusinessModel.Sprint SelectedSprint { get; set; }

        public Server SelectedServer { get; set; }

        public List<Issue> Issues { get; set; }
    }
}
