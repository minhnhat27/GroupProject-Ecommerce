using GroupProject_Ecommerce.Data;
using GroupProject_Ecommerce.Models;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GroupProject_Ecommerce.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
		private readonly MyDbContext _context;
		public HomeController(MyDbContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			var viewModel = new HomeViewModel
			{
				Categories = _context.Categories.ToList(),
				ImagesWithProducts = _context.Images
					.Include(img => img.Product)
					.ToList()
			};

			return View(viewModel);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
