using AutoUpdaterDotNET;
using MahApps.Metro.Controls;
using SprintPlanner.Core;
using SprintPlanner.Core.Logic;
using System;
using System.IO;
using System.Windows;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    /// <summary>
    /// Interaction logic for MainPlannerWindow.xaml
    /// </summary>
    public partial class MainPlannerWindow : MetroWindow
    {
        private readonly IHttpRequester _webRequester;

        public MainPlannerWindow()
        {
            InitializeComponent();
            bool isOffline = false;
#if OFFLINE
            isOffline = true;
#endif

            _webRequester = isOffline ? new CachingHttpRequester("requestCache.json") : new SimpleHttpRequester();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AutoUpdater.Start(Settings.Default.AppcastUrl);

            if (_webRequester is CachingHttpRequester cr)
            {
                try
                {
                    cr.Load();
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Cache file not found");
                }

            }

            Business.Jira = new JiraWrapper(_webRequester);

            var vm = new MainPlannerWindowViewModel(this);
            vm.Load();
            DataContext = vm;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            if (_webRequester is CachingHttpRequester cr)
            {
                cr.FlushCacheToDisk();
            }

            if (DataContext is MainPlannerWindowViewModel vm)
            {
                if (vm.MainViewModel is IStorageManipulator m)
                {
                    m.PushData();
                }

                ClearTempLogins();

                vm.Persist();
            }
        }

        private void ClearTempLogins()
        {
            foreach (var server in Business.AppData.ServerModel.Servers)
            {
                if (!server.StoreCredentials) 
                {
                    server.Logout();
                }
            }
        }
    }
}
