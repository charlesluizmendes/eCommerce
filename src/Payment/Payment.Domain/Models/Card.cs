using System.ComponentModel.DataAnnotations;

namespace Payment.Domain.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public string ClientName { get; set; }
        [Required]
        public string DateValidate { get; set; }
        [Required]
        public int SecurityCode { get; set; }
        public Payment Payment { get; set; }
    }
}
