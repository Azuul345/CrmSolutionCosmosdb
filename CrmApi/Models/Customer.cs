using Newtonsoft.Json;

namespace CrmApi.Models;

public class Customer
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

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

    [JsonProperty("partitionKey")]
    public string PartitionKey => Name?.ToLower()[..1] ?? "default";
}