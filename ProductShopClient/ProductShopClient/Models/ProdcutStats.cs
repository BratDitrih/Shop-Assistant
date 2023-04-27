using Newtonsoft.Json;
using System.Text.Json;

namespace ProductShopClient
{
    public class ProdcutStats
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("stores_amount")]
        public int StoresAmount { get; set; }

        [JsonProperty("min_price")]
        public double MinPrice { get; set; }

        [JsonProperty("max_price")]
        public double MaxPrice { get; set; }

        [JsonProperty("avg_price")]
        public double AvgPrice { get; set; }

        public override string ToString() => $"Count: {Count}, Stores Amount: {StoresAmount}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, AvgPrice: {AvgPrice}";
    }
}
