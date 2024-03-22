using GroupProject_Ecommerce.Data;
using GroupProject_Ecommerce.Models;
using GroupProject_Ecommerce.Services;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;
using webapi.Services;

namespace GroupProject_Ecommerce.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly MyDbContext _DbContext;
        private readonly ISendMailService _sendMailService;
		private readonly IVnPayService _vnPayService;

		public CartController(MyDbContext myDbContext, ISendMailService sendMailService, 
            IVnPayService vnPayService)
        {
            _DbContext = myDbContext;
            _sendMailService = sendMailService;
            _vnPayService = vnPayService;
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
            var u = await _DbContext.Users.FindAsync(userId);
            ViewBag.Name = u.FirstName + " "  + u.LastName;
            ViewBag.Phone = u.PhoneNumber;

            return View(listItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckoutCOD(IFormCollection forms)
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

                var address = forms["address"] + ", " + forms["ward"] + ", "
                    + forms["district"] + ", " + forms["city"];
                var rec = forms["phone"] + " " + forms["name"];
                var order = new Order
                {
                    UserId = userId,
                    DeliveryStatusName = "Đang xử lý",
                    Date = DateTime.Now,
                    ShippingCost = 0,
                    Total = total,
                    PayMethodName = "COD",
                    DeliveryAddress = address,
                    Receiver = rec,
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

                return RedirectToAction("PaymentSuccess", "Cart");
            }
            catch
            {
                return RedirectToAction("ShoppingCart");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckoutVNPay(IFormCollection forms)
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

            var address = forms["address"] + ", " + forms["ward"] + ", "
                    + forms["district"] + ", " + forms["city"];
            var rec = forms["phone"] + " " + forms["name"];

            var order = new Order
            {
                UserId = userId,
                DeliveryStatusName = "Đang xử lý",
                Date = DateTime.Now,
                ShippingCost = 0,
                Total = total,
                Receiver = rec,
                DeliveryAddress = address,
                Paid = false,
                PayMethodName = "VNPay",
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


            var vnPayModel = new VnPaymentRequestModel
            {
                Amount = total,
                CreatedDate = DateTime.Now,
                OrderId = order.Id,
            };

			return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
        }

		// Lưu ý do phương thức này sử dụng URL của VNPAY nên đừng thêm HttpPost
        // hay HttpGet và ValidateAntiForgeryToken ở phía trước nó sẽ khiến nó hoạt động không được
		public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var subject = "MAGIC SHOP - Thanh toán thất bại (VNPay)";
                var htmlMessage =
                    "<p>Xin chào!,</p>\r\n   " +
                    "<p>Cảm ơn bạn đã đặt hàng tại cửa hàng của chúng tôi. Đơn hàng của bạn đã được nhận và đang được xử lý.</p>" +
                    "<p>Vui lòng thanh toán lại sớm nhất có thể. Đơn hàng sẽ bị hủy sau 1 ngày nếu không được thanh toán</p>" +
                    "<p>Chúc bạn một ngày tốt lành❤️</p>" +
                "<br>" +
                "<p>Thân mến," +
                    "<p>Magic Shop</p>";
                await _sendMailService.SendEmailAsync(email, subject, htmlMessage);
                return RedirectToAction("OrderList", "User");
            }
            else
            {
                try
                {
                    var index = response.OrderDescription.LastIndexOf(":") + 1;
                    var orderId = int.Parse(response.OrderDescription.Substring(index));
                    var order = await _DbContext.Orders.SingleOrDefaultAsync(e => e.Id == orderId);
                    order.Paid = true;
                    await _DbContext.SaveChangesAsync();

                    // Gửi email xác nhận đặt hàng
                    var email = User.FindFirstValue(ClaimTypes.Email);
                    var subject = "MAGIC SHOP - Xác nhận đặt hàng (phương thức thanh toán VNPay)";
                    var htmlMessage =
                        "<p>Xin chào!,</p>\r\n   " +
                        "<p>Cảm ơn bạn đã đặt hàng tại cửa hàng của chúng tôi. Đơn hàng của bạn đã được nhận và đang được xử lý.</p>" +
                        "<p>Chúng tôi hy vọng bạn sẽ hài lòng về sản phẩm.</p>" +
                        "<p>Chúc bạn một ngày tốt lành❤️</p>" +
                        "<br>" +
                        "<p>Thân mến," +
                        "<p>Magic Shop</p>";
                    await _sendMailService.SendEmailAsync(email, subject, htmlMessage);
                    return RedirectToAction("PaymentSuccess", "Cart");
                }
                catch
                {
                    throw new Exception();
                }
            }
		}


        public IActionResult PaymentSuccess()
        {
            return View();
        }

    }
}
