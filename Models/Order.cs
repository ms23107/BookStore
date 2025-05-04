namespace BookStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();

        public int OrderTotal { get; set; }

        public DateTime OrderPlaced { get; set; }

        public string? UserEmail { get; set; }

        public string OrderStatus { get; set; } = "Apstrāde"; // Default status

    }
}
