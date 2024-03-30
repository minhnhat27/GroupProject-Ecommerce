using GroupProject_Ecommerce.Models;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using GroupProject_Ecommerce.Data;
using GroupProject_Ecommerce.Helpers;

namespace GroupProject_Ecommerce.Controllers
{
    [AllowAnonymous]
	public class AccountController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
        private readonly HttpClient _httpClient;
        private readonly MyDbContext _context;

        
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, HttpClient httpClient, MyDbContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
            _httpClient = httpClient;
            _context = context;
        }

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model)
		{
            var captcha = Request.Form["g-Recaptcha-Response"];
            var resultValidateCaptcha = ReCaptcha.Validate(captcha).Result;

            if (ModelState.IsValid && resultValidateCaptcha)
			{
				var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
				if (result.Succeeded)
				{
					if (User.IsInRole("Admin"))
					{
                        return RedirectToAction("Index", "Products", new { area = "Admin" });
                    }
					return RedirectToAction("Index", "Home");
				}
				ModelState.AddModelError("", "Invalid login attempt");
				return View(model);

			}
            if(string.IsNullOrEmpty(captcha))
                ModelState.AddModelError("ReCaptcha", "ReCaptcha is required");

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
					TempData["Register"] = "Register";
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

        public async Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });

        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                var claims = result.Principal.Identities.FirstOrDefault().Claims;
                var id = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var r = await _signInManager.ExternalLoginSignInAsync("Google", id , isPersistent: false, bypassTwoFactor: false);
                if (r.Succeeded)
                {
                    //Tài khoản có liên kết Google

                    var user = await _userManager.FindByLoginAsync("Google", id);
                    if (user != null)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    //Tài khoản chưa tồn tại, thêm tài khoản và liên kết
                    var Email = claims.First(c => c.Type == ClaimTypes.Email).Value;
                    var FirstName = claims.First(c => c.Type == ClaimTypes.GivenName).Value;
                    var LastName = claims.First(c => c.Type == ClaimTypes.Surname).Value;
                    return RedirectToAction("ExternalInfo", new { email = Email, fName = FirstName, lName = LastName, id = id });
                    //var newUser = new User
                    //{
                    //    UserName = claims.First(c => c.Type == ClaimTypes.Email).Value,
                    //    Email = claims.First(c => c.Type == ClaimTypes.Email).Value,
                    //    City = "Can Tho",
                    //    FirstName = "FN",
                    //    LastName = "LN"
                    //};
                    //var kq = await _userManager.CreateAsync(newUser);
                    //UserLoginInfo info = new UserLoginInfo("Google", id, "Google");
                    //var kq1 = await _userManager.AddLoginAsync(newUser, info);
                    //if (kq.Succeeded && kq1.Succeeded)
                    //{
                    //    // Đăng nhập người dùng mới vào hệ thống
                    //    await _signInManager.SignInAsync(newUser, isPersistent: false);

                    //    // Chuyển hướng người dùng đến trang sau khi đăng nhập thành công
                    //    return RedirectToAction("Index", "Home");
                    //}
                }
                //var claims = result.Principal.Identities.FirstOrDefault().Claims;
                //var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                //if (emailClaim != null)
                //{
                //    // Kiểm tra xem email đã tồn tại trong cơ sở dữ liệu hay chưa
                //    var existingUser = await _userManager.FindByEmailAsync(emailClaim.Value);
                //    if (existingUser != null)
                //    {
                //        await _signInManager.SignInAsync(existingUser, isPersistent: false);
                //        return RedirectToAction("Index", "Home");
                //    }
                //    else
                //    {
                //        //User user = new User
                //        //{
                //        //    Email = claims.First(c => c.Type == ClaimTypes.Email).ToString(),
                //        //    FirstName = claims.First(c => c.Type == ClaimTypes.GivenName).ToString(),
                //        //    LastName = claims.First(c => c.Type == ClaimTypes.Surname).ToString()
                //        //};
                //        //TempData["LoginInfo"] = user;
                //        //var Email = claims.First(c => c.Type == ClaimTypes.Email).Value;
                //        //var FirstName = claims.First(c => c.Type == ClaimTypes.GivenName).Value;
                //        //var LastName = claims.First(c => c.Type == ClaimTypes.Surname).Value;
                //        //return RedirectToAction("ExternalInfo", new {email = Email, fName = FirstName, lName = LastName});                        
                //    }
                //}
            }
            return RedirectToAction("Login");

        }

        public IActionResult ExternalInfo(string email, string fName, string lName, string id)
        {
            ExternalLoginModel model = new ExternalLoginModel
            {
                Id = id,
                Email = email,
                FirstName = fName,
                LastName = lName,
                PhoneNumber = "",
                City = ""
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ExternalInfo(ExternalLoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    CreateTime = DateTime.Now,
                    City = model.City 
                };
                var kq = await _userManager.CreateAsync(user);
                UserLoginInfo info = new UserLoginInfo("Google", model.Id, "Google");
                var kq1 = await _userManager.AddLoginAsync(user, info);
                if (kq.Succeeded && kq1.Succeeded)
                {
                    // Đăng nhập người dùng mới vào hệ thống
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

    }
}
