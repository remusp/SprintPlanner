using MahApps.Metro.Controls;
using SprintPlanner.Core;
using SprintPlanner.WpfApp.Properties;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            //_webRequester = new CachingHttpRequester("requestCache.json");
            _webRequester = new SimpleHttpRequester();
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
            DataContext = vm;
            vm.EnsureLoggedIn();
            vm.Load();

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
                vm.Persist();
            }
        }

        private void ListBox_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (GetScrollViewer(sender as DependencyObject) is ScrollViewer scrollViwer)
            {
                if (e.Delta < 0)
                {
                    scrollViwer.ScrollToVerticalOffset(scrollViwer.VerticalOffset + 80);//TODO: scrollspeed setting
                }
                else if (e.Delta > 0)
                {
                    scrollViwer.ScrollToVerticalOffset(scrollViwer.VerticalOffset - 80);
                }
            }
        }


        public static DependencyObject GetScrollViewer(DependencyObject o)
        {
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }
            return null;
        }
    }
}
