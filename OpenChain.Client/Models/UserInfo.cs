using Newtonsoft.Json;

namespace OpenChain.Client
{
    public class UserInfo
    {
        [JsonProperty("display_name")]
        public string DisplayName
        {
            get;
            set;
        }
    }
}