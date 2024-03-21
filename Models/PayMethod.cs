namespace GroupProject_Ecommerce.Models
{
    public class PayMethod
    {
       public int Id { get; set; }
       public string Name { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
