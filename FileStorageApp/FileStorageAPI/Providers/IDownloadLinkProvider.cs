using System;
using System.Threading.Tasks;

namespace FileStorageAPI.Providers
{
    /// <summary>
    /// Поставщик ссылки на скачивавние файла из физического хранилища
    /// </summary>
    public interface IDownloadLinkProvider
    {
        /// <summary>
        /// Возвращает строку, по которой можно скачать запрашиваемый файл
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <returns></returns>
        Task<string> GetDownloadLink(Guid id);
    }
}