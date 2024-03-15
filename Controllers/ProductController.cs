using GroupProject_Ecommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupProject_Ecommerce.Controllers
{
	public class ProductController : Controller
	{
		private readonly MyDbContext _dbContext;
		public ProductController(MyDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> ProductDetail(int id)
		{
			var product = await _dbContext.Products
				.Include(e => e.Images)
				.Include(e => e.Category)
				.SingleOrDefaultAsync(e => e.Id == id);
			if (product == null)
			{
				return NotFound();
			}
			else
			{
				return View(product);
			}
		}
	}
}
