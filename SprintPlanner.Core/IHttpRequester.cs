namespace SprintPlanner.Core
{
    public interface IHttpRequester
    {
        string HttpGetByWebRequest(string uri, string username, string password);

        byte[] HttpGetBinaryByWebRequest(string uri, string username, string password);
    }
}
