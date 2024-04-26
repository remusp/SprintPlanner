using System.Security;

namespace SprintPlanner.Core
{
    public interface IHttpRequester
    {
        string HttpGetByWebRequest(string uri, string username, SecureString password);

        byte[] HttpGetBinaryByWebRequest(string uri, string username, SecureString password);
        
        byte[] HttpGetBinaryByWebRequest(string uri);

        string HttpPut(string uri, string data, string username, SecureString password);
    }
}
