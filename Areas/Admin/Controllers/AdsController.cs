using GroupProject_Ecommerce.Data;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using webapi.Services;

namespace GroupProject_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class AdsController : Controller
    {
        private readonly MyDbContext _dbContext;
        private readonly ISendMailService _sendMailService;
        public AdsController(MyDbContext dbContext, ISendMailService sendMailService)
        {
            _dbContext = dbContext;
            _sendMailService = sendMailService;
        }

        public ActionResult Index()
        {
            ViewData["Products"] = _dbContext.Products.Select(e => new { e.Id, e.Name }).ToList();
            ViewData["Emails"] = new SelectList(_dbContext.Users, "Email", "Email");
            ViewData["Sales"] = _dbContext.Products.Where(e => e.DiscountPercent > 0).Select(p => p.Id).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Preview([Bind("Products,Title,Body,Receivers")] SendMailModel mailModel)
        {
            var listProduct = _dbContext.Products
                .Where(e => mailModel.Products.Contains(e.Id))
                .Include(e => e.Images)
                .ToList();
            var model = new PreviewMailModel
            {
                Title = mailModel.Title,
                Body = mailModel.Body,
                Products = listProduct,
                Receivers = mailModel.Receivers
            };
            TempData["Title"] = mailModel.Title;
            TempData["Body"] = mailModel.Body;
            TempData["Products"] = JsonConvert.SerializeObject(mailModel.Products);
            TempData["Receivers"] = JsonConvert.SerializeObject(mailModel.Receivers);
            return View(model);
        }

        public async Task<IActionResult> SendMail()
        {
            try
            {
                string? b = TempData["Body"] as string;
                string? title = TempData["Title"] as string;

                var body = $"<div><div>{b}</div><table><tbody>";

                List<int>? p = JsonConvert.DeserializeObject<List<int>>(TempData["Products"] as string);
                List<string> receivers = JsonConvert.DeserializeObject<List<string>>(TempData["Receivers"] as string);

                var listProduct = _dbContext.Products
                    .Where(e => p.Contains(e.Id))
                    .Include(e => e.Images)
                    .ToList();

                List<string> pictures = new List<string>();
                var i = 0;
                foreach (var item in listProduct)
                {
                    var total = item.Price - item.Price * (item.DiscountPercent / 100);
                    var strHref = "https://localhost:44322/Product/ProductDetail/" + item.Id;
                    //var strPicture = "cid:~/images/Product/" + item.Images.First().Url;
                    var strPicture = $"cid:Logo{i}.jpg";
                    i++;
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Product", item.Images.First().Url);
                    pictures.Add(imagePath);

                    body += $"<tr><td><img width='100' height='100' src='{strPicture}'/></td>" +
                        $"<td style='padding: 0 15px'>{item.Name}</td>" +
                        $"<td><div><span style='text-decoration: line-through'>{item.Price.ToString("#,##")}</span>" +
                        $"<span style='color: red'>-{item.DiscountPercent}%</span></div>" +
                        $"<div>Chỉ còn: <span style='color: red'>{total.ToString("#,##")}</span></div>" +
                        $"<div style='margin-top: 3px'><a style='text-decoration:none; border: 1px solid; padding: 4px; border-radius: 2px; background-color: cyan' href={strHref}>Mua ngay</a></div></td></tr>";
                }
                body += "</tbody></table></div>";
                
                await _sendMailService.SendEmailAsync(receivers, title, body, pictures);
                return RedirectToAction("Index", "Orders");
            }
            catch
            {
                return RedirectToAction("Index");
            }
            
        }


    }
}
