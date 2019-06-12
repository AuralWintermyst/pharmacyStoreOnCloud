using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DDAC.Models
{
    public class Drugs
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "drugname")]
        public string drugname { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string description { get; set; }

        [JsonProperty(PropertyName = "price")]
        public int price { get; set; }

        [JsonProperty(PropertyName = "imageURL")]
        public string imageURL { get; set; }

        [JsonProperty(PropertyName = "stock")]
        public int stock { get; set; }

    }
}
