using System.ComponentModel.DataAnnotations;

namespace Payment.Domain.Models
{
    public class Pix
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }
        public Payment Payment { get; set; }
    }
}
