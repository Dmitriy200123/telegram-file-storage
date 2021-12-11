namespace FileStorageAPI.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITelegramService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        RequestResult<string> LogIn(string token);
    }
}