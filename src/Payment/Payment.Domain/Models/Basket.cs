namespace Payment.Domain.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<Item> Items { get; set; }
    }
}
