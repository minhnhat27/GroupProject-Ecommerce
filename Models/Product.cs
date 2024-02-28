namespace GroupProject_Ecommerce.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public float Size { get; set; }
        public float Price { get; set; }
        public int Inventory { get; set; }
        public bool Enable { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int MaterialId { get; set; }

        public Brand Brand { get; set; }
        public Category Category { get; set; }
        public Material Material { get; set; }
        public ICollection<Image> Images { get; set; }
        public ICollection<CartItem> CartItems { get; set; }

    }
}
