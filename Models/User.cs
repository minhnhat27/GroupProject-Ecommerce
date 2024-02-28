using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace GroupProject_Ecommerce.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public bool Enable { get; set; }
        public DateTime CreateTime { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Password { get; set; }
        
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
