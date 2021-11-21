using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    /// <inheritdoc />
    public class SenderConverter : ISenderConverter
    {
        private readonly IMapper _senderMapper = new MapperConfiguration(cfg => cfg.CreateMap<FileSender, Sender>())
            .CreateMapper();

        /// <inheritdoc />
        public Sender ConvertFileSender(FileSender fileSender)
        {
            return _senderMapper.Map<Sender>(fileSender);
        }

        /// <inheritdoc />
        public List<Sender> ConvertFileSenders(List<FileSender> fileSender)
        {
            return fileSender.Select(ConvertFileSender).ToList();
        }
    }
}