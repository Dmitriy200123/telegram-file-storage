namespace DocumentsIndex.Config
{
    public class ElasticConfig : IElasticConfig
    {
        public string Uri { get; }
        public string Index { get; }

        public ElasticConfig(string uri, string index)
        {
            Uri = uri;
            Index = index;
        }
    }
}