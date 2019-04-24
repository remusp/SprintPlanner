using MahApps.Metro.Controls;
using System.Windows;

namespace SprintPlanner.WpfApp.UI.Capacity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CapacityWindow : MetroWindow
    {
        private int _boardId;
        private int _sprintId;

        public CapacityWindow(int boardId, int sprintId)
        {
            InitializeComponent();
            _boardId = boardId;
            _sprintId = sprintId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = new CapacityWindowViewModel(){ BoardId = _boardId, SprintId = _sprintId};
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
