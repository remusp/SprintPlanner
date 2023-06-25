
using SprintPlanner.Core.BusinessModel;

namespace SprintPlanner.Core.Logic
{
    public static class Business
    {
        public static JiraWrapper Jira { get; set; }

        public static DataStorageModel AppData { get; set; }

        public static SprintPlan Plan { get; set; }
    }
}
