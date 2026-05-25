using Newtonsoft.Json;

namespace CrmApi.Models;

public class Seller
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }
}