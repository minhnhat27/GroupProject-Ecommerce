namespace GroupProject_Ecommerce.Models
{
    public class DeliveryStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
