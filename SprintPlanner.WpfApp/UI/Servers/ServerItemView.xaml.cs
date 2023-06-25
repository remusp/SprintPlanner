using SprintPlanner.WpfApp.Infrastructure;
using System.Security;
using System.Windows.Controls;

namespace SprintPlanner.WpfApp.UI.Servers
{
    /// <summary>
    /// Interaction logic for ServerItemView.xaml
    /// </summary>
    public partial class ServerItemView : UserControl, IHavePassword
    {
        public ServerItemView()
        {
            InitializeComponent();
        }

        public SecureString Password => userPassword.SecurePassword;
    }
}
