using System.ComponentModel.DataAnnotations;

namespace GroupProject_Ecommerce.Models
{
    public class DeliveryStatus
    {
        [Key]
        public string Name { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
