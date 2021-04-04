using SprintPlanner.WpfApp.UI.Servers;

namespace SprintPlanner.WpfApp.DesignTime
{
    public static class DesignData
    {
        public static ServerItemViewModel ServerItemViewModelDesign => new ServerItemViewModel
        {
            ServerName = "My test Server Name",
            Url = "https://www.mysuperurl.com/link",
            StoryPointsField = "customfield_1234",
            IsActive = true
        };
    }
}
