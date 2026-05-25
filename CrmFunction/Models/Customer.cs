using Newtonsoft.Json;

namespace CrmFunction.Models;

public class Customer
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("address")]
    public string Address { get; set; }

    [JsonProperty("responsibleSeller")]
    public Seller ResponsibleSeller { get; set; }
}