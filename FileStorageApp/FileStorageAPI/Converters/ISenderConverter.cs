using System.Collections.Generic;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    /// <summary>
    /// Конвертор для преобразования отправителей в API-контракты.
    /// </summary>
    public interface ISenderConverter
    {
        /// <summary>
        /// Конвертирует информацию об отправителе в API-контракт.
        /// </summary>
        /// <param name="fileSender">Информация об отправителе из базы данных</param>
        Sender ConvertFileSender(FileSender fileSender);

        /// <summary>
        /// Конвертирует информацию об отправителях в API-контракты.
        /// </summary>
        /// <param name="fileSender">Информация об отправителях из базы данных</param>
        List<Sender> ConvertFileSenders(List<FileSender> fileSender);
    }
}