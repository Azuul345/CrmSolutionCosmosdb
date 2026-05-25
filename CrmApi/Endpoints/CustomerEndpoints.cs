using CrmApi.Models;
using CrmApi.Repositories;

namespace CrmApi.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {
        app.MapGet("/customers", async (CustomerRepository repo) =>
            await repo.GetAllAsync());

        app.MapGet("/customers/{id}", async (string id, CustomerRepository repo) =>
        {
            var customer = await repo.GetByIdAsync(id);
            return customer is null ? Results.NotFound() : Results.Ok(customer);
        });

        app.MapGet("/customers/search/name/{name}", async (string name, CustomerRepository repo) =>
            await repo.SearchByNameAsync(name));

        app.MapGet("/customers/search/seller/{sellerName}", async (string sellerName, CustomerRepository repo) =>
            await repo.SearchBySellerAsync(sellerName));

        app.MapPost("/customers", async (Customer customer, CustomerRepository repo) =>
        {
            await repo.InsertAsync(customer);
            return Results.Created($"/customers/{customer.Id}", customer);
        });

        app.MapPut("/customers/{id}", async (string id, Customer customer, CustomerRepository repo) =>
        {
            customer.Id = id;
            await repo.UpdateAsync(customer);
            return Results.Ok(customer);
        });

        app.MapDelete("/customers/{id}", async (string id, CustomerRepository repo) =>
        {
            await repo.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}

