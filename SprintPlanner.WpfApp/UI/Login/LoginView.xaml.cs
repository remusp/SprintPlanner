using SprintPlanner.WpfApp.Infrastructure;
using System.Security;
using System.Windows.Controls;

namespace SprintPlanner.WpfApp.UI.Login
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginView : UserControl, IHavePassword
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public SecureString Password => userPassword.SecurePassword;
    }
}
