namespace SprintPlanner.Core
{
    public class WebCacheEntry
    {
        public string Request { get; set; }

        public string Response { get; set; }

        public byte[] BinaryResponse { get; set; }

        public bool IsNew { get; set; }
    }
}
