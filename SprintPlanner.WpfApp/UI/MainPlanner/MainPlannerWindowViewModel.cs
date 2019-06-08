using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using SprintPlanner.Core.Logic;
using SprintPlanner.WpfApp.Properties;
using SprintPlanner.WpfApp.UI.About;
using SprintPlanner.WpfApp.UI.Capacity;
using SprintPlanner.WpfApp.UI.Login;
using SprintPlanner.WpfApp.UI.Planning;
using SprintPlanner.WpfApp.UI.SettingsUI;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class MainPlannerWindowViewModel : ViewModelBase
    {

        private readonly CapacityViewModel _capacityViewModel;

        private readonly PlanningViewModel _planningViewModel;

        private readonly LoginViewModel _loginViewModel;

        private readonly AboutViewModel _aboutViewModel;

        private readonly SettingsViewModel _settingsViewModel;

        private readonly MetroWindow _window;

        public MainPlannerWindowViewModel(MetroWindow w)
        {
            _window = w;

            LogoutVisibility = Visibility.Collapsed;

            _planningViewModel = new PlanningViewModel(w);
            _capacityViewModel = new CapacityViewModel();
            _settingsViewModel = new SettingsViewModel();
            _loginViewModel = new LoginViewModel(w);
            _loginViewModel.LoginSucceeded += LoginSucceededHandler;
            var assembly = Assembly.GetExecutingAssembly();
            _aboutViewModel = new AboutViewModel()
            {
                ProductName = ((AssemblyTitleAttribute)assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title,
                ProductVersion = assembly.GetName().Version.ToString()
            };
        }



        private ICommand _capacityViewCommand;

        public ICommand CapacityViewCommand
        {
            get
            {
                return _capacityViewCommand ?? (_capacityViewCommand = new RelayCommand(CapacityViewCommandExecute));
            }
        }

        private ICommand _aboutViewCommand;

        public ICommand AboutViewCommand
        {
            get
            {
                return _aboutViewCommand ?? (_aboutViewCommand = new RelayCommand(AboutViewCommandExecute));
            }
        }

        private ICommand _settingsViewCommand;

        public ICommand SettingsViewCommand
        {
            get
            {
                return _settingsViewCommand ?? (_settingsViewCommand = new RelayCommand(SettingsViewCommandExecute));
            }
        }

        public void Load()
        {
            const string fileName = "StoredData.json";
            if (File.Exists(fileName))
            {
                Business.Data = JsonConvert.DeserializeObject<DataStorageModel>(File.ReadAllText(fileName));
            }
        }

        #region LoggedInUserPictureData Property

        private byte[] _loggedInUserPictureData;
        public byte[] LoggedInUserPictureData
        {
            get
            {
                return _loggedInUserPictureData;
            }

            set
            {
                _loggedInUserPictureData = value;
                RaisePropertyChanged();
            }
        }

        #endregion LoggedInUserPictureData Property

        private ViewModelBase _mainViewModel;

        public ViewModelBase MainViewModel
        {
            get { return _mainViewModel; }
            set
            {
                _mainViewModel = value;
                RaisePropertyChanged();
            }
        }

        public void Persist()
        {
            string serialized = JsonConvert.SerializeObject(Business.Data, Formatting.Indented);
            File.WriteAllText("StoredData.json", serialized);
        }



        public void EnsureLoggedIn()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.User) || string.IsNullOrWhiteSpace(Settings.Default.Pass))
            {
                SetView(_loginViewModel);
            }
            else
            {
                // TODO: duplicate login
                bool isLoggedIn = Business.Jira.Login(Settings.Default.User, Settings.Default.Pass);
                if (isLoggedIn)
                {
                    LoggedInUserPictureData = Business.Jira.GetPicture(Settings.Default.User);
                    LogoutVisibility = Visibility.Visible;
                }

                SetView(_planningViewModel);

            }
        }

        private void LoginSucceededHandler()
        {
            // TODO: duplicate login
            bool isLoggedIn = _loginViewModel.IsLoggedIn;
            if (isLoggedIn)
            {
                LoggedInUserPictureData = Business.Jira.GetPicture(Settings.Default.User);
                LogoutVisibility = Visibility.Visible;
            }

            SetView(_planningViewModel);
        }

        private void CapacityViewCommandExecute()
        {
            SetView(_capacityViewModel);
        }

        private void AboutViewCommandExecute()
        {
            SetView(_aboutViewModel);
        }

        private void SettingsViewCommandExecute()
        {
            SetView(_settingsViewModel);
        }

        private ICommand _logoutCommand;

        public ICommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand = new RelayCommand(LogoutExecute));
            }
        }

        private ICommand _planningViewCommand;

        public ICommand PlanningViewCommand
        {
            get
            {
                return _planningViewCommand ?? (_planningViewCommand = new RelayCommand(PlanningViewCommandExecute));
            }
        }

        private ICommand _loginViewCommand;

        public ICommand LoginViewCommand
        {
            get
            {
                return _loginViewCommand ?? (_loginViewCommand = new RelayCommand(LoginViewCommandExecute));
            }
        }


        #region IsLogoutVisible Property

        private Visibility _logoutVisibility;

        public Visibility LogoutVisibility
        {
            get
            {
                return _logoutVisibility;
            }

            set
            {
                _logoutVisibility = value;
                RaisePropertyChanged();
            }
        }

        #endregion IsLogoutVisible Property


        private void LogoutExecute()
        {
            Settings.Default.User = string.Empty;
            Settings.Default.Pass = string.Empty;
            Settings.Default.StoreCredentials = false;
            Settings.Default.Save();

            LoggedInUserPictureData = null;
            Business.Jira.Logout();
            LogoutVisibility = Visibility.Collapsed;
            SetView(_loginViewModel);
        }

        private void PlanningViewCommandExecute()
        {
            SetView(_planningViewModel);
        }

        private void LoginViewCommandExecute()
        {
            SetView(_loginViewModel);
        }

        private void SetView(ViewModelBase vm)
        {
            if (MainViewModel is IStorageManipulator m)
            {
                m.Flush();
            }

            (vm as IStorageManipulator)?.Pull();
            MainViewModel = vm;
        }
    }
}
