using Openchain;
using System;

namespace OpenChain.Client
{
    public class BaseRecord
    {
        public ByteString Key
        {
            get;
        }

        protected ByteString OriginalEncodedValue
        {
            get;
            set;
        }

        protected ByteString EncodedValue
        {
            get;
            set;
        }

        public ByteString Version
        {
            get;
        }

        public string Path
        {
            get;
        }

        public string Type
        {
            get;
        }

        public string Name
        {
            get;
        }

        public BaseRecord(Record record)
        {
            Key = record.Key;
            var s = Key.DecodeAsString();
            var a = s.Split(':');
            if (a.Length != 3)
                throw new InvalidOperationException($"Key {s} is not a valid path");
            Path = a[0];
            Type = a[1];
            Name = a[2];
            OriginalEncodedValue = record.Value;
            EncodedValue = record.Value;
            Version = record.Version;
        }
    }
}