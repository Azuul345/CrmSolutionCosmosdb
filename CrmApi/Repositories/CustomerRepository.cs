using CrmApi.Models;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Net.Http;

namespace CrmApi.Repositories;

public class CustomerRepository
{
    private readonly Container _container;

    public CustomerRepository(IConfiguration config)
    {
        var options = new CosmosClientOptions
        {
            HttpClientFactory = () => new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }),
            ConnectionMode = ConnectionMode.Gateway,
            LimitToEndpoint = true
        };

        var client = new CosmosClient(
            config["CosmosDb:Uri"],
            config["CosmosDb:PrimaryKey"],
            options);

        var database = client.GetDatabase("CrmDB");
        _container = database.GetContainer("Customers");
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        var query = "SELECT * FROM c";
        var iterator = _container.GetItemQueryIterator<Customer>(new QueryDefinition(query));
        var results = new List<Customer>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<Customer?> GetByIdAsync(string id)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
            .WithParameter("@id", id);

        var iterator = _container.GetItemQueryIterator<Customer>(query);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            var customer = response.FirstOrDefault();
            if (customer != null)
            {
                return customer;
            }
        }

        return null;
    }

    public async Task<List<Customer>> SearchByNameAsync(string name)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE CONTAINS(LOWER(c.name), LOWER(@name))")
            .WithParameter("@name", name);
        var iterator = _container.GetItemQueryIterator<Customer>(query);
        var results = new List<Customer>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<List<Customer>> SearchBySellerAsync(string sellerName)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE CONTAINS(LOWER(c.responsibleSeller.name), LOWER(@name))")
            .WithParameter("@name", sellerName);
        var iterator = _container.GetItemQueryIterator<Customer>(query);
        var results = new List<Customer>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task InsertAsync(Customer customer) =>
        await _container.CreateItemAsync(customer, new PartitionKey(customer.PartitionKey));

    public async Task UpdateAsync(Customer customer) =>
        await _container.UpsertItemAsync(customer, new PartitionKey(customer.PartitionKey));

    public async Task DeleteAsync(string id)
    {
        var customer = await GetByIdAsync(id);
        if (customer != null)
            await _container.DeleteItemAsync<Customer>(id, new PartitionKey(customer.PartitionKey));
    }
}