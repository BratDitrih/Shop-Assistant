using Newtonsoft.Json;
using System;
using System.Text.Json;

namespace ProductShopClient
{
    public class Purchase
    {
        [JsonProperty("sale_id")]
        public int Id { get; set; }

        [JsonProperty("sale_date")]
        public DateTime SaleDate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }
    }
}
