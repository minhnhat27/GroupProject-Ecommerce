using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GroupProject_Ecommerce.Models
{
    [PrimaryKey(nameof(UserId))]
    public class Address
    {
        // public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string DiaChi { get; set; }
    }
}
