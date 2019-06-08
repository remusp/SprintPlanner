using MahApps.Metro.Controls;
using SprintPlanner.Core;
using SprintPlanner.Core.Logic;
using SprintPlanner.WpfApp.Properties;
using System.IO;
using System.Windows;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    /// <summary>
    /// Interaction logic for MainPlannerWindow.xaml
    /// </summary>
    public partial class MainPlannerWindow : MetroWindow
    {
        private IHttpRequester _webRequester;
        public MainPlannerWindow()
        {
            InitializeComponent();
            _webRequester = new CachingHttpRequester("requestCache.json");
            //_webRequester = new SimpleHttpRequester();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
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

            Business.Jira = new JiraWrapper(_webRequester) { Url = Settings.Default.Server };


            var vm = new MainPlannerWindowViewModel(this);
            vm.Load();
            DataContext = vm;
            vm.EnsureLoggedIn();

        }

        private void MetroWindow_Closed(object sender, System.EventArgs e)
        {
            if (_webRequester is CachingHttpRequester cr)
            {
                cr.FlushCacheToDisk();
            }

            if (!Settings.Default.StoreCredentials)
            {
                Settings.Default.User = string.Empty;
                Settings.Default.Pass = string.Empty;
                Settings.Default.Save();
            }

            if (DataContext is MainPlannerWindowViewModel vm)
            {
                if (vm.MainViewModel is IStorageManipulator m)
                {
                    m.Flush();
                }

                vm.Persist();
            }
        }


    }
}
