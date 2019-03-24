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
    public class PurchaseOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.POS.IRepository _pos;

        public PurchaseOrderController(ApplicationDbContext context, Services.POS.IRepository pos)
        {
            _context = context;
            _pos = pos;
        }

        // GET: PurchaseOrder
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PurchaseOrder.Include(p => p.Vendor);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PurchaseOrder/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrder
                .Include(p => p.Vendor)
                .FirstOrDefaultAsync(m => m.PurchaseOrderId == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // GET: PurchaseOrder/Create
        public IActionResult Create()
        {
            ViewData["VendorId"] = new SelectList(_context.Vendor, "VendorId", "Name");
            ViewData["Number"] = _pos.GeneratePONumber();
            return View();
        }

        // POST: PurchaseOrder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PurchaseOrderId,Number,Description,PurchaseOrderDate,VendorId")] PurchaseOrder purchaseOrder)
        {
            if (ModelState.IsValid)
            {
                purchaseOrder.PurchaseOrderId = Guid.NewGuid();
                _context.Add(purchaseOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = purchaseOrder.PurchaseOrderId });
            }
            ViewData["VendorId"] = new SelectList(_context.Vendor, "VendorId", "Name", purchaseOrder.VendorId);
            return View(purchaseOrder);
        }

        // GET: PurchaseOrder/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrder.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            ViewData["VendorId"] = new SelectList(_context.Vendor, "VendorId", "Name", purchaseOrder.VendorId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "Name");
            return View(purchaseOrder);
        }

        // POST: PurchaseOrder/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PurchaseOrderId,Number,Description,PurchaseOrderDate,VendorId")] PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.PurchaseOrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderExists(purchaseOrder.PurchaseOrderId))
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
            ViewData["VendorId"] = new SelectList(_context.Vendor, "VendorId", "Name", purchaseOrder.VendorId);
            return View(purchaseOrder);
        }

        // GET: PurchaseOrder/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrder
                .Include(p => p.Vendor)
                .FirstOrDefaultAsync(m => m.PurchaseOrderId == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // POST: PurchaseOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var purchaseOrder = await _context.PurchaseOrder.FindAsync(id);
            List<PurchaseOrderLine> line = await _context.PurchaseOrderLine.Where(x => x.PurchaseOrderId.Equals(id)).ToListAsync();
            _context.PurchaseOrderLine.RemoveRange(line);
            _context.PurchaseOrder.Remove(purchaseOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseOrderExists(Guid id)
        {
            return _context.PurchaseOrder.Any(e => e.PurchaseOrderId == id);
        }
    }
}
