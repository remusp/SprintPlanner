using System;
using System.Collections.Generic;
using System.Text;

namespace SprintPlanner.WpfCore
{
    public class Value
    {
        public int id { get; set; }
        public string self { get; set; }
        public string state { get; set; }
        public string name { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime completeDate { get; set; }
        public int originBoardId { get; set; }
        public string goal { get; set; }
    }

    public class BoardSprintsDTO
    {
        public int maxResults { get; set; }
        public int startAt { get; set; }
        public bool isLast { get; set; }
        public List<Value> values { get; set; }
    }
}
