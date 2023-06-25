using SprintPlanner.Core.BusinessModel;
using SprintPlanner.WpfApp.UI.Servers;
using System.Collections.ObjectModel;

namespace SprintPlanner.WpfApp.DesignTime
{
    public static class DesignData
    {
        public static ServerItemViewModel ServerItemViewModelDesign => new ServerItemViewModel(new Server
        {
            Name = "My test Server Name",
            Url = "https://www.mysuperurl.com/link",
            StoryPointsField = "customfield_1234",
        }, null)
        {
            UserName = "grgrg.usr",
            IsDoingLogin = false,
            IsLoggedIn = true,
            UserDisplayName = "The George",
            UserEmail = "grg@mail.com",
        };

        public static ServersViewModel ServersViewModelDesign => new ServersViewModel(null)
        {
            Servers = new ObservableCollection<ServerItemViewModel>
            {
                new ServerItemViewModel(new Server
                {
                    Name = "My test Server Name",
                    Url = "https://www.google.com/",
                    StoryPointsField = "customfield_1234",
                }, null),
                new ServerItemViewModel(new Server
                {
                    Name = "My test Server Name Super Duper Default",
                    Url = "https://www.mysuperurl.com/link",
                    StoryPointsField = "customfield_1234",
                }, null)
                {
                    IsDoingLogin = true,
                    StoreCredentials = true,
                    UserName = "superuser",
                },
                new ServerItemViewModel(new Server
                {
                    Name = "Some JIRA",
                    Url = "https://www.mysuperurl.com/jira/specific/value",
                    StoryPointsField = "customfield_12341234123412341234",
                }, null)
                {
                    IsDoingLogin = true,
                    StoreCredentials = false
                }
            }
        };
    }
}
