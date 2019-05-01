using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SprintPlanner.WpfApp.Properties;
using System;
using System.Windows;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Login
{
    public class LoginWindowViewModel : ViewModelBase
    {
        private Window _window;
        public LoginWindowViewModel(Window w)
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

        private void LoginCommandExecute()
        {
            Settings.Default.StoreCredentials = StoreCredentials;

            if (StoreCredentials)
            {
                Settings.Default.User = UserName;
                Settings.Default.Pass = Password;
            }
            else
            {
                Settings.Default.User = string.Empty;
                Settings.Default.Pass = string.Empty;
            }

            Settings.Default.Save();
            Business.Jira.Login(UserName, Password);
            _window.Close();
        }
    }
}
