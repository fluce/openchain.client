using Openchain;
using System.Threading.Tasks;

namespace OpenChain.Client
{
    public static class ApiProxyHelper
    {
        public static async Task<Record> GetValue(this ApiProxy api, string path, string type, string name, ByteString version = null)
        {
            return await api.GetValue($"{path}:{type}:{name}", version);
        }

        public static async Task<DecodedRecord<T>> GetValue<T>(this ApiProxy api, string path, string type, string name, ByteString version = null)where T : class
        {
            return new DecodedRecord<T>(await api.GetValue($"{path}:{type}:{name}", version));
        }

        public static Record AsCheckOnlyRecord(this Record record)
        {
            if (record.Version.Value.Count == 0)
                return null;
            return new Record(record.Key, null, record.Version);
        }
    }
}