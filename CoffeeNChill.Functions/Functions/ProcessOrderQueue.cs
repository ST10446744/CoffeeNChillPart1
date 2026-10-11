using System.Text.Json;
using Azure.Data.Tables;
using CoffeeNChill.Models;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeNChill.Functions
{
    public class ProcessOrderQueue
    {
        private readonly TableClient _tableClient;

        public ProcessOrderQueue()
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? "UseDevelopmentStorage=true";

            _tableClient = new TableClient(
                connectionString,
                "Orders"
            );

            _tableClient.CreateIfNotExists();
        }

        [Function("ProcessOrderQueue")]
        public async Task Run(
            [QueueTrigger("order-processing-queue", Connection = "AzureWebJobsStorage")]
            string queueMessage)
        {
            try
            {
                Console.WriteLine("Order received from queue.");

                var order = JsonSerializer.Deserialize<Order>(queueMessage);

                if (order == null)
                {
                    Console.WriteLine("Unable to read order message.");
                    return;
                }

                string orderDate =
                    DateTime.Parse(order.OrderTimestamp)
                    .ToString("yyyy-MM-dd");

                var entity = new OrderTableEntity
                {
                    PartitionKey = orderDate,
                    RowKey = order.OrderId,
                    OrderId = order.OrderId,
                    CustomerName = order.CustomerName,
                    SelectedItemSKUs =
                        string.Join(",", order.SelectedItemSKUs),
                    TotalPrice = order.TotalPrice,
                    OrderTimestamp = order.OrderTimestamp,
                    Status = "Received"
                };

                await _tableClient.UpsertEntityAsync(entity);

                Console.WriteLine(
                    $"Order {order.OrderId} status: Received");

                await Task.Delay(1000);

                entity.Status = "Preparing";
                await _tableClient.UpsertEntityAsync(entity);

                Console.WriteLine(
                    $"Order {order.OrderId} status: Preparing");

                await Task.Delay(1000);

                entity.Status = "Ready";
                await _tableClient.UpsertEntityAsync(entity);

                Console.WriteLine(
                    $"Order {order.OrderId} status: Ready");

                await Task.Delay(1000);

                entity.Status = "Collected";
                await _tableClient.UpsertEntityAsync(entity);

                Console.WriteLine(
                    $"Order {order.OrderId} status: Collected");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error processing order: {ex.Message}");

                throw;
            }
        }
    }
}