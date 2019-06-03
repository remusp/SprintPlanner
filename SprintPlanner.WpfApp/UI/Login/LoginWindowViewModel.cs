using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SprintPlanner.WpfApp.Properties;
using System;
using System.Windows;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Login
{
    public class LoginWindowViewModel : ViewModelBase
    {
        private MetroWindow _window;
        public LoginWindowViewModel(MetroWindow w)
        {
            _window = w;
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged();
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged();
            }
        }

        private bool _storeCredentials;

        public bool StoreCredentials
        {
            get { return _storeCredentials; }
            set
            {
                _storeCredentials = value;
                RaisePropertyChanged();
            }
        }

        private ICommand _loginCommand;

        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand(LoginCommandExecute);
                }

                return _loginCommand;
            }
        }


        #region IsLoggedIn Property

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get
            {
                return _isLoggedIn;
            }

            set
            {
                _isLoggedIn = value;
                RaisePropertyChanged();
            }
        }

        #endregion IsLoggedIn Property


        private void LoginCommandExecute()
        {
            Settings.Default.StoreCredentials = StoreCredentials;

            IsLoggedIn = Business.Jira.Login(UserName, Password);
            if (IsLoggedIn)
            {
                // Always store credentials, as a workaround to transmit them to main window
                Settings.Default.User = UserName;
                Settings.Default.Pass = Password;
                Settings.Default.Save();

                _window.Close();
            }
            else
            {
                _window.ShowMessageAsync("Sprint Organizer", "Username or password is wrong.");
            }

        }
    }
}
