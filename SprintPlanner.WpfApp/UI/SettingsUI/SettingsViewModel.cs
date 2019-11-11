using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using SprintPlanner.WpfApp.Properties;
using System.IO;
using System.Windows.Input;

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

        private string _sprintDataPath;

        public string SprintDataPath
        {
            get { return _sprintDataPath; }
            set
            {
                _sprintDataPath = value;
                RaisePropertyChanged();
            }
        }

        private ICommand _browseCommand;

        public ICommand BrowseCommand
        {
            get
            {
                return _browseCommand ?? (_browseCommand = new RelayCommand(BrowseCommandExecute));
            }
        }


        public void Pull()
        {
            ServerAddress = Settings.Default.Server;
            StoryPointsField = Settings.Default.StoryPointsField;
            SprintDataPath = Settings.Default.SprintDataFolder;
        }

        public void Flush()
        {
            Settings.Default.Server = ServerAddress;
            Settings.Default.StoryPointsField = StoryPointsField;
            Settings.Default.SprintDataFolder = SprintDataPath;
            Settings.Default.Save();
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
