namespace Payment.Domain.Models
{
    public class Order
    {
        public int PaymentId { get; set; }
        public Basket Basket { get; set; }
        public double Total { get; set; }
    }
}

