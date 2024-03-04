using Microsoft.AspNetCore.Mvc;

namespace GroupProject_Ecommerce.Controllers
{
	public class UserController : Controller
	{
		public IActionResult Profile()
		{
			return View();
		}
	}
}
