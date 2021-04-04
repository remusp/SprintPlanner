using SprintPlanner.FrameworkWPF;

namespace SprintPlanner.WpfApp.UI.Servers
{
    public class ServerItemViewModel : ViewModelBase
    {
        public string ServerName
        {
            get { return Get(() => ServerName); }
            set { Set(() => ServerName, value); }
        }

        public string Url
        {
            get { return Get(() => Url); }
            set { Set(() => Url, value); }
        }

        public string StoryPointsField
        {
            get { return Get(() => StoryPointsField); }
            set { Set(() => StoryPointsField, value); }
        }

        public bool IsActive
        {
            get { return Get(() => IsActive); }
            set { Set(() => IsActive, value); }
        }
    }
}
