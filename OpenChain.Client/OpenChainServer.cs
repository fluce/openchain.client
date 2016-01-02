using NBitcoin;
using Openchain;
using System.Threading.Tasks;

namespace OpenChain.Client
{
    public class OpenChainServer
    {
        public ApiProxy Api
        {
            get;
        }

        public OpenChainServer(string baseUrl)
        {
            Api = new ApiProxy(baseUrl);
        }

        public OpenChainServer(ApiProxy api)
        {
            Api = api;
        }

        static Network openChainNetwork = Network.RegisterAdditionnalNetwork(Network.Main.Name, x =>
        {
            x.Name = "openchain";
            x.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new[]{(byte)76};
            x.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new[]{(byte)78};
            x.Magic = 0;
            return x;
        }

        );
        public static Network OpenChainNetwork
        {
            get
            {
                return openChainNetwork;
            }
        }

        public OpenChainSession Login(string passphrase)
        {
            return new OpenChainSession(Api, passphrase);
        }

        public async Task<LedgerInfo> GetLedgerInfo()
        {
            var rec = await Api.GetValue<LedgerInfo>("/", "DATA", "info");
            return rec.Value;
        }

        public async Task<DecodedRecord<T>> GetData<T>(string path, string name)where T : class
        {
            return await Api.GetValue<T>(path, "DATA", name);
        }

        public async Task<TransactionInfo> SetData<T>(DecodedRecord<T> value, ExtKey key)where T : class
        {
            return await Api.PostMutation(Api.BuildMutation(ByteString.Empty, value), key);
        }



        public async Task<TransactionInfo> Transfert(string from, string to, long amount, string asset, ExtKey key)
        {
            if (from.StartsWith("@")) from = $"/aka/{from.Substring(1)}/";
            if (to.StartsWith("@")) to = $"/aka/{to.Substring(1)}/";

            var gotorec = await Api.GetValue(to, "DATA", "goto");
            var gotovalue = gotorec.Value.DecodeAsString();
            if (!string.IsNullOrEmpty(gotovalue))
                to = gotovalue;

            var acc1 = new AccountRecord(await Api.GetValue(to, "ACC", asset));
            var acc2 = new AccountRecord(await Api.GetValue(from, "ACC", asset));
            acc1.Amount += amount;
            acc2.Amount -= amount;
            return await Api.PostMutation(Api.BuildMutation(ByteString.Empty, acc1, acc2, gotorec.AsCheckOnlyRecord()), key);
        }
    }
}