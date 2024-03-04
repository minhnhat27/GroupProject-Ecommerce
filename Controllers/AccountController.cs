using GroupProject_Ecommerce.Models;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupProject_Ecommerce.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Login(LoginModel model)
		{
			//_signInManager.PasswordSignInAsync()
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Register()
		{
			return View();
		}
	}
}
