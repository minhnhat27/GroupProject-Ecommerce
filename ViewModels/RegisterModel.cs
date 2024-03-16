using System.ComponentModel.DataAnnotations;

namespace GroupProject_Ecommerce.ViewModels
{
    public class RegisterModel
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? UserName { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.MultilineText)]
        public string? City { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare("Password",ErrorMessage ="Password don't match.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

    }
}
