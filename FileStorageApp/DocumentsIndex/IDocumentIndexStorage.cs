using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentsIndex.Model;

namespace DocumentsIndex
{
    public interface IDocumentIndexStorage
    {
        Task<IEnumerable<Guid>> SearchBySubstringAsync(string subString);

        Task<bool> IndexDocumentAsync(Document document);

        Task<bool> DeleteAsync(Guid guid);
        Task<IEnumerable<Guid>> SearchByNameAsync(string name);
    }
}