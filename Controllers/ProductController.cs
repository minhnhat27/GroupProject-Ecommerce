using Microsoft.AspNetCore.Mvc;

namespace GroupProject_Ecommerce.Controllers
{
	public class ProductController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
