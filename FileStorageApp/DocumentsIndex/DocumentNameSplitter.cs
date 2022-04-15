namespace DocumentsIndex
{
    internal static class DocumentNameSplitter
    {
        public static string ReplaceDelimitersToWhiteSpaces(this string name)
        {
            var words = name.Split(',', ';', '_', '-', '.', '!', '?', '/', '\\');
            return string.Join(" ", words);
        }
    }
}