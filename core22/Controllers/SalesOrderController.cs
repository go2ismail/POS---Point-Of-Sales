using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using core22.Data;
using core22.Models.POS.Models;

namespace core22.Controllers
{
    public class SalesOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.POS.IRepository _pos;

        public SalesOrderController(ApplicationDbContext context, Services.POS.IRepository pos)
        {
            _context = context;
            _pos = pos;
        }

        // GET: SalesOrder/POS
        public IActionResult POS()
        {
            return View();
        }

        // GET: SalesOrder
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SalesOrder.Include(s => s.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SalesOrder/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrder
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(m => m.SalesOrderId == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // GET: SalesOrder/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "Name");
            ViewData["Number"] = _pos.GenerateSONumber();
            return View();
        }

        // POST: SalesOrder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalesOrderId,Number,Description,SalesOrderDate,CustomerId")] SalesOrder salesOrder)
        {
            if (ModelState.IsValid)
            {
                salesOrder.SalesOrderId = Guid.NewGuid();
                _context.Add(salesOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = salesOrder.SalesOrderId });
            }
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "Name", salesOrder.CustomerId);
            return View(salesOrder);
        }

        // GET: SalesOrder/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrder.FindAsync(id);
            if (salesOrder == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "Name", salesOrder.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "Name");
            return View(salesOrder);
        }

        // POST: SalesOrder/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SalesOrderId,Number,Description,SalesOrderDate,CustomerId")] SalesOrder salesOrder)
        {
            if (id != salesOrder.SalesOrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesOrderExists(salesOrder.SalesOrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "Name", salesOrder.CustomerId);
            return View(salesOrder);
        }

        // GET: SalesOrder/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrder
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(m => m.SalesOrderId == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // POST: SalesOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var salesOrder = await _context.SalesOrder.FindAsync(id);
            List<SalesOrderLine> line = await _context.SalesOrderLine.Where(x => x.SalesOrderId.Equals(id)).ToListAsync();
            List<InvenTran> tran = await _context.InvenTran.Where(x => x.TranSourceNumber.Equals(salesOrder.Number)).ToListAsync();
            _context.InvenTran.RemoveRange(tran);
            _context.SalesOrderLine.RemoveRange(line);
            _context.SalesOrder.Remove(salesOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesOrderExists(Guid id)
        {
            return _context.SalesOrder.Any(e => e.SalesOrderId == id);
        }
    }
}
