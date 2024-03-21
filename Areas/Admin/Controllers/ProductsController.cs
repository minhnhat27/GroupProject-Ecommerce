using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GroupProject_Ecommerce.Data;
using GroupProject_Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;

namespace GroupProject_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(MyDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Products.Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Material)
                .Include(p => p.Images);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Material)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["MaterialId"] = new SelectList(_context.Materials, "Id", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Size,Price,DiscountPercent,Inventory,Enable,BrandId,CategoryId,MaterialId")] Product product,
                                                       [Bind("files")] List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var folder = Path.Combine(_webHostEnvironment.WebRootPath, "images/Product");
                        var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        var filePath = Path.Combine(folder, fileName);

                        var image = new Image
                        {
                            Product = product,
                            ProductId = product.Id,
                            Url = fileName,
                        };
                        product.Images.Add(image);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            ViewData["MaterialId"] = new SelectList(_context.Materials, "Id", "Id", product.MaterialId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(e => e.Images)
                .SingleAsync(e => e.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["MaterialId"] = new SelectList(_context.Materials, "Id", "Name", product.MaterialId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Size,Price,DiscountPercent,Inventory,Enable,BrandId,CategoryId,MaterialId")] Product product,
                                                           [Bind("files")] List<IFormFile> files)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(files != null && files.Count > 0)
                    {
                        var lstImage = _context.Images.Where(x => x.ProductId == product.Id);
                        if(lstImage.Any())
                        {
                            _context.Images.RemoveRange(lstImage);
                            foreach(var image in lstImage)
                            {
                                var path = Path.Combine(_webHostEnvironment.WebRootPath, "images/Product");
                                var pathFile = Path.Combine(path, image.Url);
                                if (System.IO.File.Exists(pathFile))
                                {
                                    System.IO.File.Delete(pathFile);
                                }
                            }
                        }
                        foreach (var file in files)
                        {
                            var folder = Path.Combine(_webHostEnvironment.WebRootPath, "images/Product");
                            var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                            var filePath = Path.Combine(folder, fileName);

                            var image = new Image
                            {
                                Product = product,
                                ProductId = product.Id,
                                Url = fileName,
                            };
                            product.Images.Add(image);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }                            
                        }
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            ViewData["MaterialId"] = new SelectList(_context.Materials, "Id", "Id", product.MaterialId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Material)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                var lstImage = _context.Images.Where(x => x.ProductId == product.Id);
                if (lstImage.Any())
                {
                    _context.Images.RemoveRange(lstImage);
                    foreach (var image in lstImage)
                    {
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "images/Product");
                        var pathFile = Path.Combine(path, image.Url);
                        if (System.IO.File.Exists(pathFile))
                        {
                            System.IO.File.Delete(pathFile);
                        }
                    }
                }
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
