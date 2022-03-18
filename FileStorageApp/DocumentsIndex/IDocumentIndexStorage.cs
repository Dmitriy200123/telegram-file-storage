using System;
using System.Threading.Tasks;
using DocumentsIndex.Model;
using Nest;

namespace DocumentsIndex
{
    public interface IDocumentIndexStorage
    {
        Task<ISearchResponse<ElasticDocument>> Search(string subString);

        Task<bool> IndexDocument(byte[] content, Guid guid);

        Task<bool> Delete(Guid guid);
    }
}