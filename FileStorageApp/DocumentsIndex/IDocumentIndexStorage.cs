using System;
using System.Threading.Tasks;

namespace DocumentsIndex
{
    public interface IDocumentIndexStorage
    {
        Task<bool> IndexDocument(Guid documentId, string text);
        Task<bool> DeleteDocument(Guid documentId);
    }
}