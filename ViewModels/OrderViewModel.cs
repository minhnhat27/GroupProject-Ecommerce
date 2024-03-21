using GroupProject_Ecommerce.Models;

namespace GroupProject_Ecommerce.ViewModels
{
	public class OrderViewModel
	{
		public int Id { get; set; }
		public double Total { get; set; }
		public string PayMethod { get; set; }
		public string Status { get; set; }
		public DateTime Date { get; set; }
		public string Images { get; set; }
	}
}
