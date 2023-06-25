using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SprintPlanner.Core;
using SprintPlanner.Core.BusinessModel;
using SprintPlanner.Core.Extensions;
using SprintPlanner.Core.Logic;
using SprintPlanner.FrameworkWPF;
using SprintPlanner.WpfApp.Infrastructure;
using System;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Servers
{
    public class ServerItemViewModel : WrappingViewModel<Server>
    {
        private readonly MetroWindow _window;

        public ServerItemViewModel(Server server, MetroWindow w) : base(server)
        {
            CommandUserLogin = new DelegateCommand(ExecuteUserLogin);
            CommandUserLogout = new DelegateCommand(ExecuteUserLogout);
            CommandServerLogin = new DelegateCommand<IHavePassword>(ExecuteServerLogin);
            _window = w;
        }

        public string Name
        {
            get { return _model.Name; }
            set { SetBackingField(() => _model.Name = value); }
        }

        public string Url
        {
            get { return _model.Url; }
            set { SetBackingField(() => _model.Url = value); }
        }

        public string StoryPointsField
        {
            get { return _model.StoryPointsField; }
            set { SetBackingField(() => _model.StoryPointsField = value); }
        }

        public bool IsDoingLogin
        {
            get { return Get(() => IsDoingLogin); }
            set { Set(() => IsDoingLogin, value); }
        }

        public bool StoreCredentials
        {
            get { return _model.StoreCredentials; }
            set { SetBackingField(() => _model.StoreCredentials = value); }
        }

        public string UserName
        {
            get { return _model.UserName; }
            set { SetBackingField(() => _model.UserName = value); }
        }

        public string UserDisplayName
        {
            get { return _model.UserDisplayName; }
            set { SetBackingField(() => _model.UserDisplayName = value); }
        }

        public string UserEmail
        {
            get { return _model.UserEmail; }
            set { SetBackingField(() => _model.UserEmail = value); }
        }

        public bool IsLoggedIn
        {
            get { return _model.IsLoggedIn; }
            set { SetBackingField(() => _model.IsLoggedIn = value); }
        }

        public byte[] UserPictureData
        {
            get { return _model.UserPictureData; }
            set { SetBackingField(() => _model.UserPictureData = value); }
        }

        public ICommand CommandUserLogin { get; }

        public ICommand CommandUserLogout { get; }

        public ICommand CommandServerLogin { get; }

        private void ExecuteUserLogin()
        {
            IsDoingLogin = !IsDoingLogin;
        }

        private void ExecuteUserLogout()
        {
            Business.Jira.Logout();
            _model.Pass = string.Empty;
            UserDisplayName = string.Empty;
            UserEmail = string.Empty;
            IsLoggedIn = false;
        }

        private void ExecuteServerLogin(IHavePassword pwdContainer)
        {
            bool isLoggedIn = false;
            try
            {
                if (string.IsNullOrWhiteSpace(Url))
                {
                    _window.ShowMessageAsync("Cannot login", "Server URL invalid.");
                    return;
                }

                bool isOffline = false;
#if OFFLINE
            isOffline = true;
#endif
                var tempJira = new JiraWrapper(isOffline);

                isLoggedIn = tempJira.Login(Url, UserName, pwdContainer.Password);
                if (isLoggedIn)
                {
                    _model.Pass = Base64Encode(pwdContainer.Password.ToPlain());
                    var assignee = tempJira.GetAssignee(UserName);
                    UserDisplayName = assignee.displayName;
                    UserEmail = assignee.emailAddress;
                    UserPictureData = tempJira.GetPicture(assignee.name);
                }
                else
                {
                    _window.ShowMessageAsync("Login failed", "Username or password is wrong.");
                }
            }
            catch (Exception ex)
            {
                _window.ShowMessageAsync($"Login failed: {ex.Message}", ex.StackTrace);
            }
            finally 
            {
                IsDoingLogin = false;
                IsLoggedIn = isLoggedIn;
            }
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
