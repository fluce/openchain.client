using Openchain;

namespace OpenChain.Client
{
    public class AccountRecord : BaseRecord, IRecordBuilder
    {
        public string Account
        {
            get
            {
                return Path;
            }
        }

        public string Asset
        {
            get
            {
                return Name;
            }
        }

        public long Amount
        {
            get;
            set;
        }

        public AccountRecord(Record record): base (record)
        {
            Amount = record.Value.DecodeAsLong();
        }

        public Record GetRecord()
        {
            return new Record($"{Account}:ACC:{Asset}".ToByteString(), Amount.ToByteString(), Version ?? ByteString.Empty);
        }

        public static implicit operator Record(AccountRecord record)
        {
            return record.GetRecord();
        }

        public override string ToString()
        {
            return $"{Account} {Amount} {Asset}";
        }
    }
}