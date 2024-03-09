using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Order.Domain.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PaymentId { get; set; }
        [Required]
        public double Total { get; set; }
        [ForeignKey("Basket")]
        public int BasketId { get; set; }
        public Basket Basket { get; set; }
    }
}
