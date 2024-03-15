using GroupProject_Ecommerce.Models;

namespace GroupProject_Ecommerce.ViewModels
{
	public class CartItemModel
	{
		public Product Product { get; set; }
		public int Quantity { get; set; }
		public string ImageURL { get; set; }
	}
}
