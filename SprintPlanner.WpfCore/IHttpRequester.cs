namespace SprintPlanner.CoreFramework
{
    public interface IHttpRequester
    {
        string HttpGetByWebRequest(string uri, string username, string password);
    }
}