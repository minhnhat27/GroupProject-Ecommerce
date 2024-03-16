using GroupProject_Ecommerce.Data;
using GroupProject_Ecommerce.Models;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GroupProject_Ecommerce.Controllers
{
	public class UserController : Controller
	{
		private readonly MyDbContext _context;
		public UserController(MyDbContext context)
		{
			_context = context;
		}
		public IActionResult Profile()
		{
			UserViewModel user = new UserViewModel();
			user.FirstName = _context.Users.First().FirstName;
			user.LastName = _context.Users.First().LastName;
			user.FullName = user.LastName + " " + user.FirstName;
			user.City = _context.Users.First().City;
			user.CreateTime = _context.Users.First().CreateTime;
			user.Email = _context.Users.First().Email;
			user.PhoneNumber = _context.Users.First().PhoneNumber;
			return View(user);
		}

		public IActionResult Edit(/*int id*/)
		{
			UserViewModel user = new UserViewModel();
			user.FirstName = _context.Users.First().FirstName;
			user.LastName = _context.Users.First().LastName;
			user.FullName = user.LastName + " " + user.FirstName;
			user.City = _context.Users.First().City;
			user.CreateTime = _context.Users.First().CreateTime;
			user.Email = _context.Users.First().Email;
			user.PhoneNumber = _context.Users.First().PhoneNumber;
			return View(user);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(/*int id, */UserViewModel model)
		{
			User user = _context.Users.First();
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

		public IActionResult ChangePassword(/*int id*/)
		{
			User user = _context.Users.First();
			ViewBag.FullName = user.LastName + " " + user.FirstName;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(/*int id, */PasswordViewModel model)
		{
			User user = _context.Users.First();
			if (user == null)
			{
				return NotFound();
			}
			if (user.PasswordHash != model.CurrentPassword)
			{
				ModelState.AddModelError("CurrentPassword", "Mật khẩu không khớp.");
				return RedirectToAction(nameof(ChangePassword));
			}
			if (model.NewPassword == null)
			{
				return RedirectToAction(nameof(ChangePassword));
			}
			if (model.NewPassword != model.ConfirmPassword)
			{
				return RedirectToAction(nameof(ChangePassword));
			}
			user.PasswordHash = model.NewPassword;
			_context.Update(user);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Profile));
		}

		public IActionResult ChangeEmail(/*int id*/)
		{
			User user = _context.Users.First();
			ViewBag.FullName = user.LastName + " " + user.FirstName;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangeEmail(/*int id, */EmailViewModel model)
		{
			User user = _context.Users.First();
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

		public IActionResult OrderList(/*int id*/)
		{
			User user = _context.Users.First();
			ViewBag.FullName = user.LastName + " " + user.FirstName;
			string id = _context.Users.First().Id;
			List<Order> orders = _context.Orders.Where(x => x.UserId == id).ToList();
			return View(orders);
		}
	}
}
