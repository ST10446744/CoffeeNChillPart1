
using System.Text.Json;
using Azure.Storage.Queues;
using CoffeeNChill.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CoffeeNChill.Functions
{
    public class QueueOrder
    {
        private readonly QueueClient _queueClient;

        public QueueOrder()
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? "UseDevelopmentStorage=true";

            _queueClient = new QueueClient(
                connectionString,
                "order-processing-queue"
            );

            _queueClient.CreateIfNotExists();
        }

        [Function("QueueOrder")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders/queue")]
            HttpRequestData req)
        {
            try
            {
                var order = await JsonSerializer.DeserializeAsync<Order>(req.Body);

                if (order == null)
                {
                    var badResponse = req.CreateResponse(
                        System.Net.HttpStatusCode.BadRequest);

                    await badResponse.WriteStringAsync("Invalid order data.");

                    return badResponse;
                }

                order.Status = "Received";

                string message = JsonSerializer.Serialize(order);

                await _queueClient.SendMessageAsync(message);

                var response = req.CreateResponse(
                    System.Net.HttpStatusCode.Accepted);

                await response.WriteAsJsonAsync(new
                {
                    message = "Order successfully added to processing queue.",
                    orderId = order.OrderId,
                    status = order.Status
                });

                return response;
            }
            catch (Exception ex)
            {
                var response = req.CreateResponse(
                    System.Net.HttpStatusCode.InternalServerError);

                await response.WriteStringAsync(
                    $"Error adding order to queue: {ex.Message}");

                return response;
            }
        }
    }
}
