using Microsoft.AspNetCore.Mvc;

namespace GroupProject_Ecommerce.Controllers
{
	public class CartController : Controller
	{
		public IActionResult ShoppingCart()
		{
			return View();
		}
	}
}
