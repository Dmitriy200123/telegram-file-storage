﻿namespace DocumentsIndex.Factories
{
    public interface IDocumentsIndexFactory
    {
        IDocumentIndexStorage CreateDocumentIndexStorage();
    }
}