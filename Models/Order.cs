using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GroupProject_Ecommerce.Models
{
    [PrimaryKey(nameof(Id))]
    public class Order
    {
        public int Id { get; set; }
        public float Total { get; set; }
        public float ShippingCost { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public string Status { get; set; }
        public DateTime Date { get; set; }

    }

}
