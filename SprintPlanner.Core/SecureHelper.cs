using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SprintPlanner.Core
{
    public static class SecureHelper
    {
        public static string ToString(SecureString secure) 
        {
            if (secure == null)
            {
                return string.Empty;
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secure);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static SecureString ToSecure(string base64) 
        {
            string str = Base64Decode(base64);

            var secure = new SecureString();
            foreach (char c in str)
            {
                secure.AppendChar(c);
            }

            return secure;
        }

        private static string Base64Decode(string data)
        {
            var encodedBytes = Convert.FromBase64String(data);
            return Encoding.UTF8.GetString(encodedBytes);
        }
    }
}
