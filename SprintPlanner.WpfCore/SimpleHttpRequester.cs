using System;
using System.IO;
using System.Net;
using System.Text;

namespace SprintPlanner.CoreFramework
{
    public class SimpleHttpRequester : IHttpRequester
    {
        public virtual string HttpGetByWebRequest(string uri, string username, string password)
        {
            //For Basic Authentication
            string authInfo = username + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.Accept = "application/json; charset=utf-8";

            request.Headers["Authorization"] = "Basic " + authInfo;

            var response = (HttpWebResponse)request.GetResponse();

            string strResponse = "";
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                strResponse = sr.ReadToEnd();
            }

            return strResponse;
        }
    }
}
