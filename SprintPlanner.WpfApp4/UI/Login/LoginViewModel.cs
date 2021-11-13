using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SprintPlanner.Core.Extensions;
using SprintPlanner.Core.Logic;
using SprintPlanner.FrameworkWPF;
using SprintPlanner.WpfApp.Infrastructure;
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
            LoginCommand = new DelegateCommand<object>((parameter) => LoginCommandExecute(parameter));
        }

        public event Action LoginSucceeded;

        public ICommand LoginCommand { get; }

        public bool StoreCredentials
        {
            get { return Get(() => StoreCredentials); }
            set { Set(() => StoreCredentials, value); }
        }

        public string UserName
        {
            get { return Get(() => UserName); }
            set { Set(() => UserName, value); }
        }

        public bool IsLoggedIn
        {
            get { return Get(() => IsLoggedIn); }
            set { Set(() => IsLoggedIn, value); }
        }

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
                Business.Jira.ServerAddress = Settings.Default.Server;
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

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}