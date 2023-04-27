using Newtonsoft.Json;
using System;
using System.Text.Json;

namespace ProductShopClient
{
    public class Customer
    {
        [JsonProperty("customer_id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("birth_date")]
        public DateTime BirthDate { get; set; }
    }
}
