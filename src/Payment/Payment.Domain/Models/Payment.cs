using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment.Domain.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int BasketId { get; set; }
        [Required]
        public double Total { get; set; }
        public Transaction Transaction { get; set; }
        [ForeignKey("Card")]
        public int? CardId { get; set; }
        public Card Card { get; set; }
        [ForeignKey("Pix")]
        public int? PixId { get; set; }
        public Pix Pix { get; set; }
    }
}
