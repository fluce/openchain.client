using Openchain;
using System;
using System.Text;

namespace OpenChain.Client
{
    public static class ByteStringHelper
    {
        public static ByteString ToByteString(this string s)
        {
            return new ByteString(Encoding.UTF8.GetBytes(s));
        }

        public static ByteString ToByteString(this byte[] s)
        {
            return new ByteString(s);
        }

        public static string DecodeAsString(this ByteString bs)
        {
            return Encoding.UTF8.GetString(bs.ToByteArray());
        }

        public static ByteString ToByteString(this long value)
        {
            byte[] b = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return new ByteString(b);
        }

        public static long DecodeAsLong(this ByteString bs)
        {
            byte[] b = bs.ToByteArray();
            if (b.Length == 0)
                return 0;
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return BitConverter.ToInt64(b, 0);
        }
    }
}