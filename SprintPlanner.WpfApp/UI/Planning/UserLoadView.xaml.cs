using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SprintPlanner.WpfApp.UI.Planning
{
    /// <summary>
    /// Interaction logic for UserLoadView.xaml
    /// </summary>
    public partial class UserLoadView : UserControl
    {
        public UserLoadView()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri) { UseShellExecute = true });
        }
    }
}
