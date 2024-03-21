using GroupProject_Ecommerce.Data;
using GroupProject_Ecommerce.Models;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using webapi.Services;

namespace GroupProject_Ecommerce.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly MyDbContext _DbContext;
        private readonly ISendMailService _sendMailService;

        public CartController(MyDbContext myDbContext, ISendMailService sendMailService)
        {
            _DbContext = myDbContext;
            _sendMailService = sendMailService;
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
                .Include(e => e.Product)
                .Include(e => e.Product.Images)
                .ToListAsync();

            return View(listItem);
        }

        [HttpGet]
        public int CountCartItem()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return 0;
            }
            string userId = user.Value;

            var quantity = _DbContext.CartItems
                .Where(e => e.UserId == userId).Count();
            return quantity;
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

        public async Task<IActionResult> Checkout()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string userId = user.Value;

            var listItem = await _DbContext.CartItems
                .Where(e => e.UserId == userId)
                .Include(e => e.Product)
                .Include(e => e.Product.Images)
                .ToListAsync();

            if (listItem.IsNullOrEmpty())
            {
                return NotFound();
            }
            return View(listItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckoutCOD()
        {
            try
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                string userId = user.Value;

                var listCart = await _DbContext.CartItems
                    .Include(ci => ci.Product)
                    .Where(ci => ci.UserId == userId)
                    .ToListAsync();

                double total = 0;
                foreach (var cartItem in listCart)
                    total += cartItem.Product.Price * cartItem.Quantity;

                var order = new Order
                {
                    UserId = userId,
                    DeliveryStatusId = 1,
                    Date = DateTime.Now,
                    ShippingCost = 0,
                    Total = total,
                    PayMethodId = 1,
                    OrderDetails = new List<OrderDetail>()
                };

                await _DbContext.Orders.AddAsync(order);
                await _DbContext.SaveChangesAsync();

                foreach (var cartItem in listCart)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        ProductCost = cartItem.Product.Price * cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price
                    };
                    await _DbContext.OrderDetails.AddAsync(orderDetail);
                }

                _DbContext.CartItems.RemoveRange(listCart);
                await _DbContext.SaveChangesAsync();

                // Gửi email xác nhận đặt hàng
                var email = User.FindFirstValue(ClaimTypes.Email);
                var subject = "MAGIC SHOP - Xác nhận đặt hàng ( phương thức thanh toán COD)";
                var htmlMessage =
                    "<p>Xin chào!,</p>\r\n   " +
                    "<p>Cảm ơn bạn đã đặt hàng tại cửa hàng của chúng tôi. Đơn hàng của bạn đã được nhận và đang được xử lý.</p>" +
                    "<p>Chúng tôi hy vọng bạn sẽ hài lòng về sản phẩm.</p>" +
                    "<p>Chúc bạn một ngày tốt lành❤️</p>" +
                    "<br>" +
                    "<p>Thân mến," +
                    "<p>Magic Shop</p>";
                await _sendMailService.SendEmailAsync(email, subject, htmlMessage);

                return RedirectToAction("Profile", "User");
            }
            catch
            {
                return RedirectToAction("ShoppingCart");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckoutVNPay()
        {
            return View();
        }
    }
}
