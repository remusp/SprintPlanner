using System.Security;

namespace SprintPlanner.WpfApp.Infrastructure
{
    public interface IHavePassword
    {
        SecureString Password { get; }
    }
}
