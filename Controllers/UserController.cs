using GroupProject_Ecommerce.Data;
using GroupProject_Ecommerce.Models;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GroupProject_Ecommerce.Controllers
{
	public class UserController : Controller
	{
		private readonly MyDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(MyDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
		{
			_context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
		public IActionResult Profile()
		{
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string userId = user.Value;
            User u = _context.Users.Where(x => x.Id == userId).First();
            UserViewModel userv = new UserViewModel();
            userv.FirstName = u.FirstName;
            userv.LastName = u.LastName;
            userv.FullName = userv.LastName + " " + userv.FirstName;
            userv.City = u.City;
            userv.CreateTime = u.CreateTime;
            userv.Email = u.Email;
            userv.PhoneNumber = u.PhoneNumber;
            return View(userv);
        }

		public IActionResult Edit()
		{
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string userId = user.Value;
            User u = _context.Users.Where(x => x.Id == userId).First();
            UserViewModel userv = new UserViewModel();
            userv.FirstName = u.FirstName;
            userv.LastName = u.LastName;
            userv.FullName = userv.LastName + " " + userv.FirstName;
            userv.City = u.City;
            userv.CreateTime = u.CreateTime;
            userv.Email = u.Email;
            userv.PhoneNumber = u.PhoneNumber;
            return View(userv);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(UserViewModel model)
		{
            var u = User.FindFirst(ClaimTypes.NameIdentifier);
            if (u == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string userId = u.Value;
            User user = _context.Users.Where(x => x.Id == userId).First();
			if (user == null)
			{
				return NotFound();
			}
			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.City = model.City;
			user.PhoneNumber = model.PhoneNumber;
			user.Email = model.Email;
			_context.Update(user);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Profile));
		}

		public IActionResult ChangePassword()
		{
            var u = User.FindFirst(ClaimTypes.NameIdentifier);
            if (u == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string userId = u.Value;
            User user = _context.Users.Where(x => x.Id == userId).First();
            ViewBag.FullName = user.LastName + " " + user.FirstName;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(PasswordViewModel model)
		{

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                // ChangePasswordAsync changes the user password
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                // The new password did not meet the complexity rules or
                // the current password is incorrect. Add these errors to
                // the ModelState and rerender ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                // Upon successfully changing the password refresh sign-in cookie
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }

		public IActionResult ChangeEmail()
		{
            var u = User.FindFirst(ClaimTypes.NameIdentifier);
            if (u == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string userId = u.Value;
            User user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            ViewBag.FullName = user.LastName + " " + user.FirstName;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangeEmail(EmailViewModel model)
		{
            var u = User.FindFirst(ClaimTypes.NameIdentifier);
            if (u == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string userId = u.Value;
            User user = _context.Users.Where(x => x.Id == userId).First();
            if (user == null)
			{
				return NotFound();
			}
			if (user.Email != model.CurrentEmail)
			{
				return RedirectToAction(nameof(ChangeEmail));
			}
			if (model.NewEmail == null)
			{
				return RedirectToAction(nameof(ChangeEmail));
			}
			user.Email = model.NewEmail;
			user.NormalizedEmail = model.NewEmail;
			_context.Update(user);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Profile));
		}

		public IActionResult OrderList()
		{
            var u = User.FindFirst(ClaimTypes.NameIdentifier);
            if (u == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string userId = u.Value;
            User user = _context.Users.Where(x => x.Id == userId).First();
            ViewBag.FullName = user.LastName + " " + user.FirstName;
			string id = user.Id;
			List<Order> orders =  _context.Orders.Where(x => x.UserId == id).ToList();
            List<OrderViewModel> ordersvm = new List<OrderViewModel>();
            foreach(Order o in orders)
            {
                OrderViewModel i = new OrderViewModel();
                i.Id = o.Id;
                i.Total = o.Total;
                i.Status = o.Status;
                i.Date = o.Date;
                i.PayMethod = o.PayMethod;
                int pId = _context.OrderDetails.Where(x => x.OrderId == o.Id).First().ProductId;
                string img = _context.Images.Where(x => x.ProductId == pId).First().Url;
				i.Images = img;
                ordersvm.Add(i);
            }
			return View(ordersvm);
		}

        public IActionResult OrderDetails(int id)
        {
			var u = User.FindFirst(ClaimTypes.NameIdentifier);
			if (u == null)
			{
				return RedirectToAction("Login", "Account");
			}
			string userId = u.Value;
			User user = _context.Users.Where(x => x.Id == userId).First();
			ViewBag.FullName = user.LastName + " " + user.FirstName;
			Order o = _context.Orders.Where(x => x.Id == id).First();
            var ords = _context.OrderDetails.Where(x => x.OrderId == id).Include(x => x.Product).Include(x => x.Product.Images).ToList();
            ViewBag.Order = o;
            return View(ords);
        }
	}
}
