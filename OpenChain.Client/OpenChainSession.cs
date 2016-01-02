using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenChain.Client
{
    public class OpenChainSession : OpenChainServer, IDisposable
    {
        ExtKey passphraseKey
        {
            get;
            set;
        }

        ExtKey rootKey
        {
            get;
            set;
        }

        public string Account
        {
            get;
            private set;
        }

        public OpenChainSession(ApiProxy api, string passphrase): base (api)
        {
            Mnemonic mnemonic = new Mnemonic(passphrase);
            passphraseKey = mnemonic.DeriveExtKey();
            rootKey = passphraseKey.Derive(44, true).Derive(64, true).Derive(0, true).Derive(0).Derive(0);
            var bprootkey = rootKey.PrivateKey.PubKey.GetAddress(OpenChainServer.OpenChainNetwork);
            Account = $"/p2pkh/{bprootkey}/";
        }

        public string GetAssetPath(uint index)
        {
            var assetKey = passphraseKey.Derive(44, true).Derive(64, true).Derive(1, true).Derive(0).Derive(index);
            var bpassetkey = assetKey.PrivateKey.PubKey.GetAddress(OpenChainServer.OpenChainNetwork);
            return $"/asset/p2pkh/{bpassetkey}/";
        }

        public async Task<TransactionInfo> SetData<T>(DecodedRecord<T> value)where T : class
        {
            return await SetData<T>(value, rootKey);
        }

        public async Task<TransactionInfo> Transfert(string from, string to, long amount, string asset)
        {
            return await Transfert(from, to, amount, asset, rootKey);
        }

        public async Task<List<AccountRecord>> GetAccountRecords()
        {
            return (await Api.GetAccountAssets(Account)).Select(x => new AccountRecord(x)).ToList();
        }

#region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    passphraseKey = null;
                    rootKey = null;
                    Account = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
#endregion
    }
}