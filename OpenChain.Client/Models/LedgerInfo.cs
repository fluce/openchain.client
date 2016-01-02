using Newtonsoft.Json;

namespace OpenChain.Client
{
    public class LedgerInfo
    {
        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("validator_url")]
        public string ValidatorUrl
        {
            get;
            set;
        }

        [JsonProperty("tos")]
        public string TermsOfService
        {
            get;
            set;
        }

        [JsonProperty("webpage_url")]
        public string WebPageUrl
        {
            get;
            set;
        }
    }
}