using GroupProject_Ecommerce.Models;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model)
		{
                    return RedirectToAction("Index", "Home");

                }
				ModelState.AddModelError("", "Invalid login attempt");
			return View(model);

            }
			//_signInManager.PasswordSignInAsync()
			return View(model);
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				User user = new()
				{
					FirstName=model.FirstName,
					LastName=model.LastName,
					UserName = model.UserName,
					PhoneNumber = model.PhoneNumber,
					Email = model.Email,
					City=model.City,
					CreateTime=DateTime.Now,
				};
				var result = await _userManager.CreateAsync(user,model.Password);
				if (result.Succeeded)
				{
					//await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Login", "Account");

                }
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
            }
			return View(model);
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

	}
}
