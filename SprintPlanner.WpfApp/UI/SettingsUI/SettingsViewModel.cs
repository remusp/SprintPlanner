using GalaSoft.MvvmLight;
using SprintPlanner.WpfApp.Properties;

namespace SprintPlanner.WpfApp.UI.SettingsUI
{
    public class SettingsViewModel : ViewModelBase, IStorageManipulator
    {
        private string _serverAddress;

        public string ServerAddress
        {
            get { return _serverAddress; }
            set
            {
                _serverAddress = value;
                RaisePropertyChanged();
            }
        }

        private string _storyPointsField;

        public string StoryPointsField
        {
            get { return _storyPointsField; }
            set
            {
                _storyPointsField = value;
                RaisePropertyChanged();
            }
        }

        public void Pull()
        {
            ServerAddress = Settings.Default.Server;
            StoryPointsField = Settings.Default.StoryPointsField;
        }

        public void Flush()
        {
            Settings.Default.Server = ServerAddress;
            Settings.Default.StoryPointsField = StoryPointsField;
            Settings.Default.Save();
        }
    }
}
