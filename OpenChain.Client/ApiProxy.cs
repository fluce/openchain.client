using NBitcoin;
using Newtonsoft.Json.Linq;
using Openchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace OpenChain.Client
{
    public class ApiProxy
    {
        string BaseUrl
        {
            get;
            set;
        }

        public ApiProxy(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        private InstanceInfo _instanceInfo;
        public InstanceInfo InstanceInfo
        {
            get
            {
                if (_instanceInfo == null)
                {
                    var t = GetInfo();
                    t.Wait();
                    _instanceInfo = t.Result;
                }

                return _instanceInfo;
            }
        }

        public async Task<InstanceInfo> GetInfo()
        {
            using (var cli = new HttpClient())
            {
                var query = $"{BaseUrl}info";
                var tresult = await cli.GetStringAsync(query);
                var jobj = JObject.Parse(tresult);
                return new InstanceInfo { Namespace = ByteString.Parse((string)jobj["namespace"]) };
            }
        }

        public async Task<Record> GetValue(string key, ByteString version = null)
        {
            using (var cli = new HttpClient())
                if (version == null)
                {
                    var query = $"{BaseUrl}record?key={key.ToByteString()}";
                    var tresult = await cli.GetStringAsync(query);
                    var jobj = JObject.Parse(tresult);
                    return new Record(key.ToByteString(), ByteString.Parse((string)jobj["value"]), ByteString.Parse((string)jobj["version"]));
                }
                else
                {
                    var query = $"{BaseUrl}query/recordversion?key={key.ToByteString()}&version={version}";
                    var tresult = await cli.GetStringAsync(query);
                    var jobj = JObject.Parse(tresult);
                    return new Record(key.ToByteString(), ByteString.Parse((string)jobj["value"]), ByteString.Parse((string)jobj["version"]));
                }
        }

        public Mutation BuildMutation(ByteString metadata, params Record[] records)
        {
            var m = new Mutation(InstanceInfo.Namespace, records.Where(x=>x!= null), metadata);
            return m;
        }

        public async Task<TransactionInfo> PostMutation(Mutation mutation, ExtKey key)
        {
            var sm = MessageSerializer.SerializeMutation(mutation).ToByteString();
            var hash = new uint256(NBitcoin.Crypto.Hashes.SHA256(NBitcoin.Crypto.Hashes.SHA256(sm.ToByteArray())));
            var msig = key.PrivateKey.Sign(hash).ToDER().ToByteString();
            var j = JObject.FromObject(new
            {
                mutation = sm.ToString(),
                signatures = new[]{
                    new
                    {
                        pub_key = key.Neuter().PubKey.ToHex(),
                        signature = msig.ToString()
                    }
                }
            });
            using (var cli = new HttpClient())
            {
                cli.Timeout = TimeSpan.FromHours(1);
                var query = $"{BaseUrl}submit";
                var tresult = await cli.PostAsync(query, new ByteArrayContent(Encoding.UTF8.GetBytes(j.ToString(Formatting.None))));
                var s = await tresult.Content.ReadAsStringAsync();
                var obj = JObject.Parse(s);
                return new TransactionInfo { MutationHash = ByteString.Parse((string)obj["mutation_hash"]), TransactionHash = ByteString.Parse((string)obj["transaction_hash"]) };
            }
        }

        public async Task<List<Record>> GetAccountAssets(string account)
        {
            using (var cli = new HttpClient())
            {
                var query = $"{BaseUrl}query/account?account={WebUtility.UrlEncode(account)}";
                var tresult = await cli.GetStringAsync(query);
                var ret = JsonConvert.DeserializeObject<QueryAccountResultItem[]>(tresult);
                return ret.Select(x => new Record($"{x.account}:ACC:{x.asset}".ToByteString(), x.balance.ToByteString(), ByteString.Parse(x.version))).ToList();
            }
        }

        private class QueryAccountResultItem
        {
            public string account
            {
                get;
                set;
            }

            public string asset
            {
                get;
                set;
            }

            public long balance
            {
                get;
                set;
            }

            public string version
            {
                get;
                set;
            }
        }
    }
}