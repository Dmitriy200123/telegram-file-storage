using System;
using System.Threading.Tasks;
using DocumentsIndex.Model;

namespace DocumentsIndex
{
    public interface IDocumentIndexStorage
    {
        Task<bool> IndexDocument(Guid documentId, string text);
        Task<bool> DeleteDocument(Guid documentId);

        ElasticDocument? GetDoc(string text);
    }
}