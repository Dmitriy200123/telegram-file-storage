namespace DocumentsIndex.Config
{
    public class ElasticConfig
    {
        public ElasticConfig(string uri, string index)
        {
            Uri = uri;
            Index = index;
        }

        public string Uri { get; }
        public string Index { get; }
    }
}