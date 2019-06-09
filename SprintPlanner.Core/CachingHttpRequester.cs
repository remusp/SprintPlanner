using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace SprintPlanner.Core
{
    public class CachingHttpRequester : SimpleHttpRequester
    {
        private readonly string _cacheFile;

        private Dictionary<string, WebCacheEntry> _webCache;

        public CachingHttpRequester(string cacheFile)
        {
            _cacheFile = cacheFile;
            _webCache = new Dictionary<string, WebCacheEntry>();
        }

        public void Load()
        {
            using (StreamReader file = File.OpenText(_cacheFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                _webCache = (Dictionary<string, WebCacheEntry>)serializer.Deserialize(file, typeof(Dictionary<string, WebCacheEntry>));
            }
        }

        public override string HttpGetByWebRequest(string uri, string username, SecureString password)
        {
            string response = string.Empty;
            if (_webCache.ContainsKey(uri))
            {
                response = _webCache[uri].Response;
            }
            else
            {
                response = base.HttpGetByWebRequest(uri, username, password);
                _webCache.Add(uri, new WebCacheEntry
                {
                    Request = uri,
                    Response = response,
                });
            }

            return response;
        }

        public override byte[] HttpGetBinaryByWebRequest(string uri, string username, SecureString password)
        {
            byte[] response = null;
            if (_webCache.ContainsKey(uri))
            {
                response = _webCache[uri].BinaryResponse;
            }
            else
            {
                response = base.HttpGetBinaryByWebRequest(uri, username, password);
                _webCache.Add(uri, new WebCacheEntry
                {
                    Request = uri,
                    BinaryResponse = response,
                });
            }

            return response;
        }

        public void FlushCacheToDisk()
        {
            using (StreamWriter file = File.CreateText(_cacheFile))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                serializer.Serialize(file, _webCache);
            }
        }
    }
}
