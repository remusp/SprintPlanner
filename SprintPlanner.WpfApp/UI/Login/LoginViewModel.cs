using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SprintPlanner.Core.Logic;
using SprintPlanner.WpfApp.Properties;
using System;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Login
{
    public class LoginViewModel : ViewModelBase, IStorageManipulator
    {
        private readonly MetroWindow _window;

        public LoginViewModel(MetroWindow w)
        {
            _window = w;
        }

        public event Action LoginSucceeded;

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
                return _loginCommand ?? (_loginCommand = new RelayCommand(LoginCommandExecute));
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
                OnLoginSucceeded();
            }
            else
            {
                _window.ShowMessageAsync("Sprint Organizer", "Username or password is wrong.");
            }

        }

        protected void OnLoginSucceeded()
        {
            LoginSucceeded?.Invoke();
        }

        public void Pull()
        {
            StoreCredentials = Settings.Default.StoreCredentials;
            if (StoreCredentials)
            {
                UserName = Settings.Default.User;
                Password = Settings.Default.Pass;
            }
        }

        public void Flush()
        {

        }


    }
}
