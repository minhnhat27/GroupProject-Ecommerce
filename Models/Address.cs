using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupProject_Ecommerce.Models
{
    public class Address
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        public string DiaChi { get; set; }
    }
}
