using Microsoft.Win32;
using SprintPlanner.FrameworkWPF;
using SprintPlanner.WpfApp.Properties;
using System.IO;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.SettingsUI
{
    public class SettingsViewModel : ViewModelBase, IStorageManipulator
    {
        public SettingsViewModel()
        {
            BrowseCommand = new DelegateCommand(BrowseCommandExecute);
        }

        public ICommand BrowseCommand { get; }

        public string ServerAddress
        {
            get { return Get(() => ServerAddress); }
            set { Set(() => ServerAddress, value); }
        }

        public string SprintDataPath
        {
            get { return Get(() => SprintDataPath); }
            set { Set(() => SprintDataPath, value); }
        }

        public string StoryPointsField
        {
            get { return Get(() => StoryPointsField); }
            set { Set(() => StoryPointsField, value); }
        }

        public void Flush()
        {
            Settings.Default.Server = ServerAddress;
            Settings.Default.StoryPointsField = StoryPointsField;
            Settings.Default.SprintDataFolder = SprintDataPath;
            Settings.Default.Save();
        }

        public void Pull()
        {
            ServerAddress = Settings.Default.Server;
            StoryPointsField = Settings.Default.StoryPointsField;
            SprintDataPath = Settings.Default.SprintDataFolder;
        }

        private void BrowseCommandExecute()
        {
            OpenFileDialog ofd = new OpenFileDialog() { InitialDirectory = SprintDataPath };
            ofd.ValidateNames = false;
            ofd.CheckFileExists = false;
            ofd.CheckPathExists = true;

            // Always default to Folder Selection.
            ofd.FileName = "Folder Selection.";

            if (ofd.ShowDialog() == true)
            {
                SprintDataPath = Path.GetDirectoryName(ofd.FileName);
            }
        }
    }
}