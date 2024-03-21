using GroupProject_Ecommerce.Models;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;

namespace GroupProject_Ecommerce.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
        private readonly HttpClient _httpClient;

        
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, HttpClient httpClient)
		{
			_userManager = userManager;
			_signInManager = signInManager;
            _httpClient = httpClient;
        }

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Home");

				}
				ModelState.AddModelError("", "Invalid login attempt");
				return View(model);

			}
			//_signInManager.PasswordSignInAsync()
			return View(model);
		}

        public async Task<IActionResult> Register()
        {
            string apiUrl = "https://vapi.vnappmob.com/api/province";
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CityResponse>(data);
                var cities = result.Results;
                ViewBag.Cities = cities;
                return View("Register");
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model, string SelectedCityName)
        {
            if (ModelState.IsValid)
            {
                User user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    CreateTime = DateTime.Now,
                    City = SelectedCityName // Lưu tên thành phố
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
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
