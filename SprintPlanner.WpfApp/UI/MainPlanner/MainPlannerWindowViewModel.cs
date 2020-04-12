﻿using GalaSoft.MvvmLight;
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
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Windows;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class MainPlannerWindowViewModel : ViewModelBase
    {
        private const string sprintFileName = "Sprint.spl.json";

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
            _capacityViewModel = new CapacityViewModel(w);
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

        private bool _isEnabledPlanning;

        public bool IsEnabledPlanning
        {
            get { return _isEnabledPlanning; }
            set
            {
                _isEnabledPlanning = value;
                RaisePropertyChanged();
            }
        }

        private bool _isEnabledCapacity;

        public bool IsEnabledCapacity
        {
            get { return _isEnabledCapacity; }
            set
            {
                _isEnabledCapacity = value;
                RaisePropertyChanged();
            }
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

            var sprintFilePath = Path.Combine(Settings.Default.SprintDataFolder, sprintFileName);
            File.WriteAllText(sprintFilePath, serialized);
        }

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

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
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
