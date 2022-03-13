namespace DocumentsIndex.Config
{
    public class ElasticConfig : IElasticConfig
    {
        public ElasticConfig(string uri)
        {
            Uri = uri;
        }

        public string Uri { get; }
    }
}