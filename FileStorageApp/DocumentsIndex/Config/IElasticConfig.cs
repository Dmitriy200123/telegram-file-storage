namespace DocumentsIndex.Config
{
    public interface IElasticConfig
    {
        string Uri { get; }
        string Index { get; }
    }
}