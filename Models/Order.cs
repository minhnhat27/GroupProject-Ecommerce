using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupProject_Ecommerce.Models
{
    public class Order
    {
        public int Id { get; set; }
        public float Total { get; set; }
        public float ShippingCost { get; set; } 

        public string UserId { get; set; }
        public User User { get; set; }

        public string Status { get; set; }
        public DateTime Date { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }


    }

}
