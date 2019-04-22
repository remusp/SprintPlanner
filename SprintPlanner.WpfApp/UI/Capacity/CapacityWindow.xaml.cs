using MahApps.Metro.Controls;
using System.Windows;

namespace SprintPlanner.WpfApp.UI.Capacity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CapacityWindow : MetroWindow
    {
        public CapacityWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = new CapacityWindowViewModel();
            DataContext = vm;
            vm.Load();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is CapacityWindowViewModel vm)
            {
                vm.Persist();
            }
        }
    }
}
