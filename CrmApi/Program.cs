using CrmApi.Endpoints;
using CrmApi.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CustomerRepository>();

var app = builder.Build();

app.MapCustomerEndpoints();

app.Run();

