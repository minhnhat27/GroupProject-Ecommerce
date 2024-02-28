using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace GroupProject_Ecommerce.Models
{
    public class User : IdentityUser
    {
        // public int Id { get; set; }
        //public bool Enable { get; set; }
        //public string Password { get; set; }
        //public string PhoneNumber { get; set; }
        //public string Email { get; set; }
        //public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public DateTime CreateTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<Order> Orders { get; set; }


    }
}
