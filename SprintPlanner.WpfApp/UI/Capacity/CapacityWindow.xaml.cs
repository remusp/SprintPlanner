using System.Windows;

namespace SprintPlanner.WpfApp.UI.Capacity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CapacityWindow : Window
    {
        public CapacityWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new CapacityWindowViewModel();
        }
    }
}
