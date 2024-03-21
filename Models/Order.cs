using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupProject_Ecommerce.Models
{
    public class Order
    {
        public int Id { get; set; }
        public double Total { get; set; }
        public float ShippingCost { get; set; } 

        public string UserId { get; set; }
        public User? User { get; set; }

        public string PayMethodName { get; set; }
        public PayMethod? PayMethod { get; set; }

        public bool Paid { get; set; } = false;

        public DateTime Date { get; set; }

        public string DeliveryStatusName { get; set; }
        public DeliveryStatus? DeliveryStatus { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

}
