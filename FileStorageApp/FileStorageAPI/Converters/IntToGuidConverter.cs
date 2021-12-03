using System;

namespace FileStorageAPI.Converters
{
    /// <inheritdoc />
    public class IntToGuidConverter : IIntToGuidConverter
    {
        /// <inheritdoc />
        public Guid Convert(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }
}