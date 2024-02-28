using Microsoft.EntityFrameworkCore;

namespace GroupProject_Ecommerce.Models
{
    [PrimaryKey(nameof(OrderId),nameof(ProductId))]
    public class OrderDetail
    {
        public int OrderId { get; set; } 
        public Order Order { get; set; } 

        public int ProductId { get; set; } 
        public Product Product { get; set; } 

        public int Quantity { get; set; } 
        public float ProductCost { get; set; } 
        public float UnitPrice { get; set; } 
    }
}
