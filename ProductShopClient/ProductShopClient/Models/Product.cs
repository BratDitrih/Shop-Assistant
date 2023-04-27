using Newtonsoft.Json;
using System;
using System.Text.Json;

namespace ProductShopClient
{
    public class Product
    {
        [JsonProperty("product_id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        public override string ToString() => $"Id: {Id}\nName: {Name}\nCategory: {Category}\nBrand: {Brand}\nPrice: {Price}\nStartDate: {StartDate}";
    }
}
