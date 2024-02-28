using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GroupProject_Ecommerce.Models
{
    public class Image
    {
        [Key, ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Url { get; set; }

    }
}
