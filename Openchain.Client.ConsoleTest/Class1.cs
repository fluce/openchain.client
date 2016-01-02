using System;
using System.Threading.Tasks;

namespace OpenChain.Client.ConsoleTest
{
    public class Class1
    {
        public Class1()
        {
        }

        // these are test-only secrets !
        const string admin = "combat pélican gagner bateau caporal infini charbon neutron détester menhir causer espoir carbone saugrenu obscur inexact torrent rayonner laisser relief féroce honteux cirer époque";
        // corresponding address is "XiqvPB63hh8TML2iWYGDvF7i3HXRxqv3nN" : add this address to admin list in openchain server config.json

        const string alice = "pélican combat gagner bateau caporal infini charbon neutron détester menhir causer espoir carbone saugrenu obscur inexact torrent rayonner laisser relief féroce honteux cirer époque";
        const string bob = "pélican gagner combat bateau caporal infini charbon neutron détester menhir causer espoir carbone saugrenu obscur inexact torrent rayonner laisser relief féroce honteux cirer époque";

        public async Task Run()
        {
            var ocs = new OpenChainServer("http://localhost:8080/");

            string assetPath;

            using (var a = ocs.Login(alice))
            using (var ad = ocs.Login(admin))
            using (var b = ocs.Login(bob))
            {
                var ir = await ocs.GetData<LedgerInfo>("/", "info");
                if (ir.Value == null || ir.Value.Name != "My Ledger")
                {
                    ir.Value = new LedgerInfo { Name = "My Ledger" };
                    await ad.SetData(ir);
                }

                var s = await ocs.GetData<string>("/", "info");

                var gt = await ad.GetData<string>("/aka/alice/", "goto");
                if (gt.Value == null)
                {
                    gt.Value = a.Account;
                    await ad.SetData(gt);
                }

                gt = await ad.GetData<string>("/aka/bob/", "goto");
                if (gt.Value == null)
                {
                    gt.Value = b.Account;
                    await ad.SetData(gt);
                }

                assetPath = "/asset/gold/"; //ad.GetAssetPath(0);

                foreach (var r in await ad.GetAccountRecords())
                    Console.WriteLine($"ad : {r}");

                Console.WriteLine($"Transfert : {await ad.Transfert(assetPath, ad.Account, 300, assetPath)}");

                foreach (var r in await ad.GetAccountRecords())
                    Console.WriteLine($"ad : {r}");

                var re = await b.GetData<UserInfo>(b.Account, "info");
                if (re.Value == null)
                {
                    re.Value = new UserInfo { DisplayName = "Bob" };
                    Console.WriteLine($"SetData : {await b.SetData(re)}");
                }

                foreach (var r in await ad.GetAccountRecords())
                    Console.WriteLine($"ad : {r}");
                foreach (var r in await b.GetAccountRecords())
                    Console.WriteLine($"b  : {r}");

                Console.WriteLine($"Transfert : {await ad.Transfert(ad.Account, b.Account, 12, assetPath)}");

                foreach (var r in await ad.GetAccountRecords())
                    Console.WriteLine($"ad : {r}");
                foreach (var r in await b.GetAccountRecords())
                    Console.WriteLine($"b  : {r}");
                foreach (var r in await a.GetAccountRecords())
                    Console.WriteLine($"a  : {r}");

                Console.WriteLine($"Transfert : {await b.Transfert(b.Account, a.Account, 2, assetPath)}");

                foreach (var r in await b.GetAccountRecords())
                    Console.WriteLine($"b  : {r}");
                foreach (var r in await a.GetAccountRecords())
                    Console.WriteLine($"a  : {r}");

                Console.WriteLine($"Transfert : {await ad.Transfert(assetPath, "@alice", 100, assetPath)}");
                Console.WriteLine($"Transfert : {await ad.Transfert(assetPath, "@bob", 33, assetPath)}");

                foreach (var r in await b.GetAccountRecords())
                    Console.WriteLine($"b  : {r}");
                foreach (var r in await a.GetAccountRecords())
                    Console.WriteLine($"a  : {r}");

            }
        }
    }
}