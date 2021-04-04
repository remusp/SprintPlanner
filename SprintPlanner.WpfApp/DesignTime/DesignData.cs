using SprintPlanner.WpfApp.UI.Servers;
using System.Collections.ObjectModel;

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

        public static ServersViewModel ServersViewModelDesign => new ServersViewModel
        {
            Servers = new ObservableCollection<ServerItemViewModel>
            {
                new ServerItemViewModel
                {
                    ServerName = "My test Server Name",
                    Url = "https://www.google.com/",
                    StoryPointsField = "customfield_1234",
                    IsActive = false
                },
                new ServerItemViewModel
                {
                    ServerName = "My test Server Name Super Duper Default",
                    Url = "https://www.mysuperurl.com/link",
                    StoryPointsField = "customfield_1234",
                    IsActive = true
                },
                new ServerItemViewModel
                {
                    ServerName = "Some JIRA",
                    Url = "https://www.mysuperurl.com/jira/specific/value",
                    StoryPointsField = "customfield_12341234123412341234",
                    IsActive = false
                }
            }
        };
    }
}
