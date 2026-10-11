using Azure;
using Azure.Data.Tables;

namespace CoffeeNChill.Models
{
    public class OrderTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty;

        public string RowKey { get; set; } = string.Empty;

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public string OrderId { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string SelectedItemSKUs { get; set; } = string.Empty;

        public double TotalPrice { get; set; }

        public string OrderTimestamp { get; set; } = string.Empty;

        public string Status { get; set; } = "Received";
    }
}