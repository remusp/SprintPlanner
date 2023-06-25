using System.Security;

namespace SprintPlanner.Core.Extensions
{
    public static class SecureStringExtensions
    {
        public static string ToPlain(this SecureString secure)
        {
            return SecureHelper.ToString(secure);
        }
    }
}
