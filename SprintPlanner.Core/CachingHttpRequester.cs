using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public void FlushCacheToDisk()
        {
            var newEntries = _webCache.Values.Where(v => v.IsNew);
            List<string> lines = new List<string>();
            foreach (var e in newEntries)
            {
                lines.Add(e.Request);
                lines.Add(e.Response);
            }

            File.AppendAllLines(_cacheFile, lines);
        }
    }
}
