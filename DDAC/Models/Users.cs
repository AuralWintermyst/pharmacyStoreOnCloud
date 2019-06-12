using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC.Models
{
    public class Users
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }


        [JsonProperty(PropertyName = "name")]
        [StringLength(30, MinimumLength = 1)]
        [Required]
        public string Name { get; set; }


        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "username")]
        [StringLength(30, MinimumLength = 1)]
        [Required]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "password")]
        //[Required(ErrorMessage = "Password is required")]
        [StringLength(30, MinimumLength = 1)]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
