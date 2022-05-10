using System;
using System.Text;
using DocumentsIndex.Contracts;

namespace SearchDocumentsAPI.Tests.Data
{
    public static class APIDocumentSearchData
    {
        private static readonly Guid FirstDocumentId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa1");
        private static readonly Guid SecondDocumentId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa2");

        public static object[] NameDocumentsSource =
        {
            new object[]
            {
                "example",
                GetEmptyDocuments("example_one.docx", "example_two.docx"),
                new[] {FirstDocumentId, SecondDocumentId}
            },
            new object[]
            {
                "docx",
                GetEmptyDocuments("example_one.docx", "example_two.png"),
                new[] {FirstDocumentId}
            },
            new object[]
            {
                "two png",
                GetEmptyDocuments("example_one.docx", "example_two.png"),
                new[] {SecondDocumentId}
            }
        };

        public static object[] ContentDocumentsSource =
        {
            new object[]
            {
                "hate nest",
                GetContentDocuments("example_one.docx", "NEST search", "example_two.docx", "Hate this"),
                new[] {FirstDocumentId, SecondDocumentId}
            },
            new object[]
            {
                "search",
                GetContentDocuments("example_one.docx", "NEST search", "example_two.docx", "Hate this"),
                new[] {FirstDocumentId}
            },
            new object[]
            {
                "Hate",
                GetContentDocuments("example_one.docx", "NEST search", "example_two.docx", "Hate this"),
                new[] {SecondDocumentId}
            }
        };
        
        public static object[] ContentOrNameDocumentsSource =
        {
            new object[]
            {
                "magic example",
                GetContentDocuments("example.docx", "NEST", "file.txt", "Magic hole"),
                new[] {FirstDocumentId, SecondDocumentId}
            },
            new object[]
            {
                "search hole",
                GetContentDocuments("example.docx", "NEST search", "file.txt", "Magic hole"),
                new[] {SecondDocumentId, FirstDocumentId}
            }
        };
        
        public static object[] EmptyDocumentsSource =
        {
            new object[]
            {
                "magic",
                GetContentDocuments("example.docx", "NEST", "file.txt", "PAIN"),
                Array.Empty<Guid>()
            },
            new object[]
            {
                "peace",
                GetContentDocuments("example.docx", "Elastic", "file.txt", "hole"),
                Array.Empty<Guid>()
            }
        };
        
        public static object[] TextsContainsInDocumentNameSource =
        {
            new object[]
            {
                new Document(FirstDocumentId, Array.Empty<byte>(), "filename.txt"),
                new[] {"name"},
                true
            },
            new object[]
            {
                new Document(FirstDocumentId, Array.Empty<byte>(), "filename.txt"),
                new[] {"empty", "name", "test"},
                true
            },
            new object[]
            {
                new Document(FirstDocumentId, Array.Empty<byte>(), "foo.txt"),
                new[] {"empty", "name", "txt"},
                true
            },
            new object[]
            {
                new Document(FirstDocumentId, Array.Empty<byte>(), "pain.docx"),
                new[] {""},
                true
            },
            new object[]
            {
                new Document(FirstDocumentId, Array.Empty<byte>(), "pain.docx"),
                new[] {"p"},
                true
            },
            new object[]
            {
                new Document(FirstDocumentId, Array.Empty<byte>(), "pain.docx"),
                new[] {"pa"},
                true
            },
            new object[]
            {
                new Document(FirstDocumentId, Array.Empty<byte>(), "pain.docx"),
                new[] {"empty", "name", "test"},
                false
            },
            new object[]
            {
                new Document(FirstDocumentId, Array.Empty<byte>(), "pain.docx"),
                new[] {"v"},
                false
            },
            new object[]
            {
                new Document(FirstDocumentId, Array.Empty<byte>(), "pain.docx"),
                new[] {"la", "vo"},
                false
            }
        };

        private static Document[] GetEmptyDocuments(string firstName, string secondName)
        {
            return new[]
            {
                new Document(FirstDocumentId, Array.Empty<byte>(), firstName),
                new Document(SecondDocumentId, Array.Empty<byte>(), secondName)
            };
        }

        private static Document[] GetContentDocuments(
            string firstName,
            string firstContent,
            string secondName,
            string secondContent
        )
        {
            return new[]
            {
                new Document(FirstDocumentId, Encoding.UTF8.GetBytes(firstContent), firstName),
                new Document(SecondDocumentId, Encoding.UTF8.GetBytes(secondContent), secondName)
            };
        }
    }
}