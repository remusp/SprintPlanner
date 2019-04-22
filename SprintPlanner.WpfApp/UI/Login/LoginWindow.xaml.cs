using MahApps.Metro.Controls;
using System.Windows;

namespace SprintPlanner.WpfApp.UI.Login
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : MetroWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new LoginWindowViewModel(this);
        }
    }
}
