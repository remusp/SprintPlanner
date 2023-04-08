using MahApps.Metro.Controls;
using Newtonsoft.Json;
using SprintPlanner.Core.Logic;
using SprintPlanner.FrameworkWPF;
using SprintPlanner.WpfApp.UI.About;
using SprintPlanner.WpfApp.UI.Capacity;
using SprintPlanner.WpfApp.UI.Login;
using SprintPlanner.WpfApp.UI.Planning;
using SprintPlanner.WpfApp.UI.SettingsUI;
using SprintPlanner.WpfApp.UI.Stats;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class MainPlannerWindowViewModel : ViewModelBase
    {
        private const string sprintFileName = "Sprint.spl.json";

        private readonly AboutViewModel _aboutViewModel;
        private readonly CapacityViewModel _capacityViewModel;
        private readonly StatsViewModel _statsViewModel;
        private readonly LoginViewModel _loginViewModel;
        private readonly PlanningViewModel _planningViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        private readonly MetroWindow _window;

        public MainPlannerWindowViewModel(MetroWindow w)
        {
            _window = w;

            CapacityViewCommand = new DelegateCommand(() => SetView(_capacityViewModel));
            StatsViewCommand = new DelegateCommand(() => SetView(_statsViewModel));
            AboutViewCommand = new DelegateCommand(() => SetView(_aboutViewModel));
            SettingsViewCommand = new DelegateCommand(() => SetView(_settingsViewModel));
            PlanningViewCommand = new DelegateCommand(() => SetView(_planningViewModel));
            LoginViewCommand = new DelegateCommand(() => SetView(_loginViewModel));
            LogoutCommand = new DelegateCommand(LogoutExecute);

            LogoutVisibility = Visibility.Collapsed;

            _planningViewModel = new PlanningViewModel(w);
            _capacityViewModel = new CapacityViewModel(w);
            _statsViewModel = new StatsViewModel();
            _settingsViewModel = new SettingsViewModel();
            _loginViewModel = new LoginViewModel(w);
            _loginViewModel.LoginSucceeded += LoginSucceededHandler;
            var assembly = Assembly.GetExecutingAssembly();
            _aboutViewModel = new AboutViewModel(w)
            {
                ProductName = ((AssemblyTitleAttribute)assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title,
                ProductVersion = assembly.GetName().Version.ToString()
            };
        }

        public ICommand AboutViewCommand { get; }

        public ICommand CapacityViewCommand { get; }
        
        public ICommand StatsViewCommand { get; }

        public bool IsEnabledCapacity
        {
            get { return Get(() => IsEnabledCapacity); }
            set { Set(() => IsEnabledCapacity, value); }
        }

        public bool IsEnabledPlanning
        {
            get { return Get(() => IsEnabledPlanning); }
            set { Set(() => IsEnabledPlanning, value); }
        }

        public byte[] LoggedInUserPictureData
        {
            get { return Get(() => LoggedInUserPictureData); }
            set { Set(() => LoggedInUserPictureData, value); }
        }

        public ICommand LoginViewCommand { get; }
        public ICommand LogoutCommand { get; }

        public Visibility LogoutVisibility
        {
            get { return Get(() => LogoutVisibility); }
            set { Set(() => LogoutVisibility, value); }
        }

        public ViewModelBase MainViewModel
        {
            get { return Get(() => MainViewModel); }
            set { Set(() => MainViewModel, value); }
        }

        public ICommand PlanningViewCommand { get; }
        public ICommand SettingsViewCommand { get; }

        public void EnsureLoggedIn()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.User) || string.IsNullOrWhiteSpace(Settings.Default.Pass))
            {
                SetView(_loginViewModel);
            }
            else
            {
                string str = Base64Decode(Settings.Default.Pass);

                var secure = new SecureString();
                foreach (char c in str)
                {
                    secure.AppendChar(c);
                }

                // TODO: duplicate login
                bool isLoggedIn = Business.Jira.Login(Settings.Default.User, secure);
                if (isLoggedIn)
                {
                    try
                    {
                        LoggedInUserPictureData = Business.Jira.GetPicture(Settings.Default.User);
                    }
                    catch (WebException wex)
                    {
                        Debug.WriteLine(wex);
                    }

                    LogoutVisibility = Visibility.Visible;
                    SetView(_planningViewModel);

                    IsEnabledPlanning = true;
                    IsEnabledCapacity = true;
                }
                else
                {
                    SetView(_loginViewModel);
                    IsEnabledPlanning = false;
                    IsEnabledCapacity = false;
                }
            }
        }

        public void Load()
        {
            var dataFolder = Settings.Default.SprintDataFolder;

            var assembly = Assembly.GetExecutingAssembly();
            var appName = ((AssemblyTitleAttribute)assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title;
            if (string.IsNullOrWhiteSpace(dataFolder))
            {
                dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), appName);
                Settings.Default.SprintDataFolder = dataFolder;
                Settings.Default.Save();

                Directory.CreateDirectory(dataFolder);
            }

            var sprintFilePath = Path.Combine(dataFolder, sprintFileName);

            if (File.Exists(sprintFilePath))
            {
                Business.Data = JsonConvert.DeserializeObject<DataStorageModel>(File.ReadAllText(sprintFilePath));
            }
            else
            {
                Business.Data = new DataStorageModel()
                {
                    Sprint = new SprintStorageModel(),
                    Capacity = new CapacityStorageModel()
                };
            }
        }

        public void Persist()
        {
            string serialized = JsonConvert.SerializeObject(Business.Data, Formatting.Indented);

            var sprintFilePath = Path.Combine(Settings.Default.SprintDataFolder, sprintFileName);
            File.WriteAllText(sprintFilePath, serialized);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private void LoginSucceededHandler()
        {
            // TODO: duplicate login
            bool isLoggedIn = _loginViewModel.IsLoggedIn;
            if (isLoggedIn)
            {
                try
                {
                    LoggedInUserPictureData = Business.Jira.GetPicture(Settings.Default.User);
                }
                catch (WebException wex)
                {
                    Debug.WriteLine(wex);
                }

                LogoutVisibility = Visibility.Visible;
                SetView(_planningViewModel);

                IsEnabledPlanning = true;
                IsEnabledCapacity = true;
            }
            else
            {
                SetView(_loginViewModel);
                IsEnabledPlanning = false;
                IsEnabledCapacity = false;
            }
        }

        private void LogoutExecute()
        {
            Settings.Default.User = string.Empty;
            Settings.Default.Pass = null;
            Settings.Default.StoreCredentials = false;
            Settings.Default.Save();

            LoggedInUserPictureData = null;
            Business.Jira.Logout();
            LogoutVisibility = Visibility.Collapsed;
            SetView(_loginViewModel);
            IsEnabledPlanning = false;
            IsEnabledCapacity = false;
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