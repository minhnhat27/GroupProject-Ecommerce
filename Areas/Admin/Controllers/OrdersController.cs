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
    public class OrdersController : Controller
    {
        private readonly MyDbContext _context;

        public OrdersController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Orders.Include(o => o.DeliveryStatus)
                .Include(o => o.PayMethod)
                .Include(o => o.User);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Admin/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(e => e.User)
                .Include(e => e.PayMethod)
                .Include(e => e.OrderDetails)
                    .ThenInclude(e => e.Product)
                        .ThenInclude( e => e.Images)
                .SingleOrDefaultAsync(e => e.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["DeliveryStatusId"] = new SelectList(_context.DeliveryStatus, "Id", "Name", order.DeliveryStatusId);
            ViewData["PayMethodId"] = new SelectList(_context.PayMethods, "Id", "Name", order.PayMethodId);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Total,ShippingCost,UserId,PayMethodId,Paid,Date,DeliveryStatusId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["DeliveryStatusId"] = new SelectList(_context.DeliveryStatus, "Id", "Id", order.DeliveryStatusId);
            ViewData["PayMethodId"] = new SelectList(_context.PayMethods, "Id", "Id", order.PayMethodId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
