using Openchain;
using Newtonsoft.Json;

namespace OpenChain.Client
{
    public class DecodedRecord<T> : BaseRecord, IRecordBuilder where T : class
    {
        public DecodedRecord(Record record): base (record)
        {
        }

        public override string ToString()
        {
            return $"{Path}:{Type}:{Name} : [{typeof (T).Name}] {GetValueAsString()}";
        }

        private bool decoded;
        private T value;
        public T Value
        {
            get
            {
                CheckDecode();
                return value;
            }

            set
            {
                this.value = value;
                decoded = true;
            }
        }

        private void CheckDecode()
        {
            if (!decoded)
            {
                if (EncodedValue == null)
                    value = null;
                else
                {
                    var v = EncodedValue.DecodeAsString();
                    if (v.Length == 0)
                        value = null;
                    else if (typeof(T)==typeof(string))
                        value = v as T;
                    else
                        value = JsonConvert.DeserializeObject<T>(v);
                }
                decoded = true;
            }
        }

        protected string GetValueAsString()
        {
            if ((typeof(T) == typeof(string)))
                return Value as string;
            return JsonConvert.SerializeObject(Value);
        }

        public Record GetRecord()
        {
            EncodedValue = Value==null?null:GetValueAsString().ToByteString();
            return new Record(Key, EncodedValue, Version);
        }

        public static implicit operator Record(DecodedRecord<T> record)
        {
            return record.GetRecord();
        }
    }
}