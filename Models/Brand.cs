namespace GroupProject_Ecommerce.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
