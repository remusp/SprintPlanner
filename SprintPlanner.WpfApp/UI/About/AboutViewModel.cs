//using AutoUpdaterDotNET;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SprintPlanner.FrameworkWPF;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.About
{
    public class AboutViewModel : ViewModelBase
    {
        private readonly MetroWindow _window;

        public AboutViewModel(MetroWindow window)
        {
            _window = window;
            CheckUpdatesCommand = new DelegateCommand(CheckUpdatesCommandExecute);
        }
        public string ProductName
        {
            get { return Get(() => ProductName); }
            set { Set(() => ProductName, value); }
        }

        public string ProductVersion
        {
            get { return Get(() => ProductVersion); }
            set { Set(() => ProductVersion, value); }
        }

        public ICommand CheckUpdatesCommand { get; }

        private void CheckUpdatesCommandExecute()
        {
            //AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent;
            //AutoUpdater.Start(Settings.Default.AppcastUrl);
        }

        //private void AutoUpdater_CheckForUpdateEvent(UpdateInfoEventArgs args)
        //{
        //    if (!args.IsUpdateAvailable)
        //    {
        //        _window.ShowMessageAsync("Application is up to date", "Already at latest version");
        //    }

        //    AutoUpdater.CheckForUpdateEvent -= AutoUpdater_CheckForUpdateEvent;

        //}
    }
}