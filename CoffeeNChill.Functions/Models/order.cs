
namespace CoffeeNChill.Models
{
    public class Order
    {
        public string OrderId { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public List<string> SelectedItemSKUs { get; set; } = new List<string>();

        public double TotalPrice { get; set; }

        public string OrderTimestamp { get; set; } = string.Empty;

        public string Status { get; set; } = "Received";
    }
}

