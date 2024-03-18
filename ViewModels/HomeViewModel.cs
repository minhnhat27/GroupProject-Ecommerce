using System.Collections.Generic;
using GroupProject_Ecommerce.Models;

namespace GroupProject_Ecommerce.ViewModels
{
	public class HomeViewModel
	{
		public List<Category> Categories { get; set; }
		public List<Image> ImagesWithProducts { get; set; }
	}
}