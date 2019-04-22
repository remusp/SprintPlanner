using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SprintPlanner.WpfApp.Properties;
using SprintPlanner.WpfApp.UI.Capacity;
using SprintPlanner.WpfApp.UI.Login;
using System;
using System.Windows;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class MainPlannerWindowViewModel : ViewModelBase
    {
        public MainPlannerWindowViewModel(Window w)
        {
            _window = w;
        }

        private ICommand openCapacityWindowCommand;

        private Window _window;

        public ICommand OpenCapacityWindowCommand
        {
            get
            {
                if (openCapacityWindowCommand == null)
                {
                    openCapacityWindowCommand = new RelayCommand(OpenCapacityWindowCommandExecute);
                }

                return openCapacityWindowCommand;
            }
        }

        public void EnsureLoggedIn()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.User) || string.IsNullOrWhiteSpace(Settings.Default.Pass))
            {
                new LoginWindow() { Owner = _window }.Show();
            }
            else
            {
                Business.Jira.Login(Settings.Default.User, Settings.Default.Pass);
            }
        }

        private void OpenCapacityWindowCommandExecute()
        {
            new CapacityWindow().Show();
        }
    }
}
