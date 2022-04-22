using System;

namespace RightServices
{
    public class RightsSettings
    {
        public byte[] Key { get;}
        public RightsSettings(byte[] key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }
}