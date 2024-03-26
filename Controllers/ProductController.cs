using GroupProject_Ecommerce.Data;
using GroupProject_Ecommerce.ViewModels;
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
			var viewModel = new HomeViewModel
			{
				Categories = _dbContext.Categories.ToList(),
				ImagesWithProducts = _dbContext.Images
					.Include(img => img.Product)
					.ToList()
			};

			return View(viewModel);
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

		[HttpGet]
		public IActionResult Search(string search)
		{
			search = search ?? string.Empty;
			var viewModel = new HomeViewModel
			{
				Categories = _dbContext.Categories.ToList(),
				ImagesWithProducts = _dbContext.Images
					.Include(img => img.Product)
					.Where(e => e.Product.Name.Contains(search))
					.ToList()
			};
			ViewBag.Search = search;

			return View(viewModel);
		}
	}
}
