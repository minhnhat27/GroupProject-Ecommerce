using GroupProject_Ecommerce.Data;
using GroupProject_Ecommerce.Models;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace GroupProject_Ecommerce.Controllers
{
	[Authorize]
	public class CartController : Controller
	{
		private readonly MyDbContext _DbContext;
		public CartController(MyDbContext myDbContext)
		{
			_DbContext = myDbContext;
		}
		public async Task<IActionResult> ShoppingCart()
		{
			var user = User.FindFirst(ClaimTypes.NameIdentifier);
			if (user == null)
			{
				return RedirectToAction("Login", "Account");
			}
			string userId = user.Value;

			var listItem = await _DbContext.CartItems
				.Where(e => e.UserId == userId)
				.Select(e => new CartItemModel
				{
					Product = e.Product,
					Quantity = e.Quantity,
					ImageURL = e.Product.Images.First().Url
				}).ToListAsync();
			ViewBag.EmptyCart = false;
			if (listItem.IsNullOrEmpty())
			{
				ViewBag.EmptyCart = true;
			}
			return View(listItem);
		}

		[HttpPost]
		public async Task<IActionResult> ChangeQuantity(int productId, int quantity)
		{
			var user = User.FindFirst(ClaimTypes.NameIdentifier);
			if (user == null)
			{
				return RedirectToAction("Login", "Account");
			}
			string userId = user.Value;

			var cartItem = await _DbContext.CartItems
				.Where(e => e.UserId == userId && e.ProductId == productId)
				.SingleOrDefaultAsync();
			if (cartItem == null || quantity < 1)
			{
				return BadRequest();
			}
			cartItem.Quantity = quantity;
			await _DbContext.SaveChangesAsync();
			return Ok();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteCartItem(int productId)
		{
			try
			{
				var user = User.FindFirst(ClaimTypes.NameIdentifier);
				if (user == null)
				{
					return RedirectToAction("Login", "Account");
				}
				string userId = user.Value;

				var cartItem = await _DbContext.CartItems
					.Where(e => e.UserId == userId && e.ProductId == productId)
					.SingleOrDefaultAsync();
				if (cartItem == null)
				{
					return RedirectToAction("Index", "Home");
				}
				_DbContext.CartItems.Remove(cartItem);
				await _DbContext.SaveChangesAsync();
				return RedirectToAction("ShoppingCart");
			}
			catch
			{
				return View("ShoppingCart");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteAllCartItem()
		{
			var user = User.FindFirst(ClaimTypes.NameIdentifier);
			if (user == null)
			{
				return RedirectToAction("Login", "Account");
			}
			string userId = user.Value;

			var cartItem = await _DbContext.CartItems
				.Where(e => e.UserId == userId)
				.ToListAsync();
			if (cartItem.IsNullOrEmpty())
			{
				return RedirectToAction("Index", "Home");
			}
			_DbContext.CartItems.RemoveRange(cartItem);
			await _DbContext.SaveChangesAsync();
			return RedirectToAction("ShoppingCart");
		}

		[HttpPost]
		public async Task<IActionResult> AddToCart(int productId, int quantity)
		{
			try
			{
				var user = User.FindFirst(ClaimTypes.NameIdentifier);
				if (user == null)
				{
					return RedirectToAction("Login", "Account");
				}
				string userId = user.Value;

				var cartItem = await _DbContext.CartItems
					.SingleOrDefaultAsync(e => e.UserId == userId && e.ProductId == productId);
				if (cartItem == null)
				{
					var model = new CartItem
					{
						UserId = userId,
						ProductId = productId,
						Quantity = quantity
					};
					await _DbContext.AddAsync(model);
				}
				else
				{
					cartItem.Quantity += quantity;
				}
				await _DbContext.SaveChangesAsync();
				return Ok();
			}
			catch
			{
				return BadRequest();
			}
		}

		public IActionResult Checkout()
		{
			return View();
		}
	}
}
