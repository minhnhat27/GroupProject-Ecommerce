using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupProject_Ecommerce.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }

        public string DiaChi { get; set; }
    }
}
