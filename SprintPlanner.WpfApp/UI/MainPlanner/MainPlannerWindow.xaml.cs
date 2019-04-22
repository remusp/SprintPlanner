using MahApps.Metro.Controls;
using SprintPlanner.WpfApp.Properties;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    /// <summary>
    /// Interaction logic for MainPlannerWindow.xaml
    /// </summary>
    public partial class MainPlannerWindow : MetroWindow
    {
        public MainPlannerWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = new MainPlannerWindowViewModel(this);
            DataContext = vm;
            vm.EnsureLoggedIn();

        }

        private void MetroWindow_Closed(object sender, System.EventArgs e)
        {
            if (!Settings.Default.StoreCredentials)
            {
                Settings.Default.User = string.Empty;
                Settings.Default.Pass = string.Empty;
            }
        }
    }
}
