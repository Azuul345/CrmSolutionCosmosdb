using CrmFunction.Models;
using CrmFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.Json;

namespace CrmFunction;

public class CustomerChangedFunction
{
    private readonly EmailService _emailService;

    public CustomerChangedFunction(EmailService emailService)
    {
        _emailService = emailService;
    }

    [Function("CustomerChangedFunction")]
    public async Task Run(
        [CosmosDBTrigger(
            databaseName: "CrmDB",
            containerName: "Customers",
            Connection = "CosmosDBConnection",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)]
        IReadOnlyList<JsonDocument> input,
        FunctionContext context)
    {
        var log = context.GetLogger("CustomerChangedFunction");

        if (input == null || input.Count == 0)
        {
            return;
        }

        foreach (var doc in input)
        {
            var json = doc.RootElement.GetRawText();

            var customer = JsonConvert.DeserializeObject<Customer>(json);

            if (customer == null)
            {
                log.LogWarning("Received a change feed item that could not be deserialized to Customer.");
                continue;
            }

            log.LogInformation($"Customer changed: {customer.Name}");
            await _emailService.SendSellerNotificationAsync(customer);
        }
    }
}