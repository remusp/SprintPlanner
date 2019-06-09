using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SprintPlanner.Core.Extensions;
using SprintPlanner.Core.Logic;
using SprintPlanner.WpfApp.Infrastructure;
using SprintPlanner.WpfApp.Properties;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Login
{
    public class LoginViewModel : ViewModelBase, IStorageManipulator
    {
        private readonly MetroWindow _window;

        private ICommand _loginCommand;

        private bool _storeCredentials;

        private string _userName;

        public LoginViewModel(MetroWindow w)
        {
            _window = w;
        }

        public event Action LoginSucceeded;

        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new RelayCommand<object>((parameter) => LoginCommandExecute(parameter)));
            }
        }

        public bool StoreCredentials
        {
            get { return _storeCredentials; }
            set
            {
                _storeCredentials = value;
                RaisePropertyChanged();
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged();
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

        public void Flush()
        {
        }

        public void Pull()
        {
            StoreCredentials = Settings.Default.StoreCredentials;
            if (StoreCredentials)
            {
                UserName = Settings.Default.User;
            }
        }

        protected void OnLoginSucceeded()
        {
            LoginSucceeded?.Invoke();
        }

        private void LoginCommandExecute(object parameter)
        {
            Settings.Default.StoreCredentials = StoreCredentials;

            if (parameter is IHavePassword passwordContainer)
            {
                IsLoggedIn = Business.Jira.Login(UserName, passwordContainer.Password);
                if (IsLoggedIn)
                {
                    // Always store credentials, as a workaround to transmit them to main window
                    Settings.Default.User = UserName;
                    Settings.Default.Pass = Base64Encode(passwordContainer.Password.Decrypt());
                    Settings.Default.Save();
                    OnLoginSucceeded();
                }
                else
                {
                    _window.ShowMessageAsync("Sprint Organizer", "Username or password is wrong.");
                }
            }
            else
            {
                _window.ShowMessageAsync("Sprint Organizer", "Unable to login. Error while reading password.");
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}