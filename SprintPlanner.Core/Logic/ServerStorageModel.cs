using SprintPlanner.Core.BusinessModel;
using System.Collections.Generic;

namespace SprintPlanner.Core.Logic
{
    public class ServerStorageModel
    {
        public ServerStorageModel()
        {
            Servers = new List<Server>();  
        }

        public List<Server> Servers { get; set; }
    }
}
