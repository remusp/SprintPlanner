using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            string[] lines = File.ReadAllLines(_cacheFile);

            for (int i = 0; i < lines.Length; i += 2)
            {
                _webCache.Add(lines[i], new WebCacheEntry
                {
                    Request = lines[i],
                    Response = lines[i + 1],
                    BinaryResponse = lines[i + 2] != "NULL" ? Encoding.ASCII.GetBytes(lines[i + 2]) : null,
                    IsNew = false
                });
            }
        }

        public override string HttpGetByWebRequest(string uri, string username, string password)
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
                    IsNew = true
                });
            }

            return response;
        }

        public override byte[] HttpGetBinaryByWebRequest(string uri, string username, string password)
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
                    IsNew = true
                });
            }

            return response;
        }

        public void FlushCacheToDisk()
        {
            var newEntries = _webCache.Values.Where(v => v.IsNew);
            var lines = new List<string>();
            foreach (WebCacheEntry e in newEntries)
            {
                lines.Add(e.Request);
                lines.Add(e.Response);
                lines.Add(Encoding.ASCII.GetString(e.BinaryResponse ?? Encoding.ASCII.GetBytes("NULL")));
            }

            File.AppendAllLines(_cacheFile, lines);
        }
    }
}
