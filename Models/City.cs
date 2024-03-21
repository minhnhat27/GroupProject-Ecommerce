using System.ComponentModel.DataAnnotations;

namespace GroupProject_Ecommerce.Models
{
    public class City
    {
        [Key]
        public string province_id { get; set; }
        public string province_name { get; set; }
        public string province_type { get; set; }
    }
}
