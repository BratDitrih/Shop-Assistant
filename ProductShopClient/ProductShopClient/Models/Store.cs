using Newtonsoft.Json;
using System.Text.Json;

namespace ProductShopClient
{
    public class Store
    {
        [JsonProperty("store_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("region")]
        public int Region { get; set; }

        public override string ToString() => $"Id: {Id}, Address: {Address}, Region: {Region}";
    }
}
