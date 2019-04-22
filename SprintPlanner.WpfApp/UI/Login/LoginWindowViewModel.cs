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

        private string userName;

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                RaisePropertyChanged();
            }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                RaisePropertyChanged();
            }
        }

        private bool storeCredentials;

        public bool StoreCredentials
        {
            get { return storeCredentials; }
            set
            {
                storeCredentials = value;
                RaisePropertyChanged();
            }
        }

        private ICommand loginCommand;

        public ICommand LoginCommand
        {
            get
            {
                if (loginCommand == null)
                {
                    loginCommand = new RelayCommand(LoginCommandExecute);
                }

                return loginCommand;
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
