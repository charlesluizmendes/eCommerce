using System.ComponentModel.DataAnnotations;

namespace Order.Domain.Models
{
    public class Basket
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public Order Order { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
