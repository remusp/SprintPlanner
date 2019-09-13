namespace SprintPlanner.Core.Logic
{
    public class IssueModel
    {
        public string StoryId { get; set; }

        public string Name { get; set; }

        public string ParentName { get; set; }

        public decimal Hours { get; set; }

        public string TaskId { get; set; }

        public string TaskLink { get; set; }

        public string StoryLink { get; set; }


    }
}
