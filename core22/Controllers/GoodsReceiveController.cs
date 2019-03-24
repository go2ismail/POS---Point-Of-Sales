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
    public class GoodsReceiveController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.POS.IRepository _pos;

        public GoodsReceiveController(ApplicationDbContext context, Services.POS.IRepository pos)
        {
            _context = context;
            _pos = pos;
        }

        // GET: GoodsReceive
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.GoodsReceive.Include(g => g.PurchaseOrder);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: GoodsReceive/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goodsReceive = await _context.GoodsReceive
                .Include(g => g.PurchaseOrder)
                .FirstOrDefaultAsync(m => m.GoodsReceiveId == id);
            if (goodsReceive == null)
            {
                return NotFound();
            }

            return View(goodsReceive);
        }

        // GET: GoodsReceive/Create
        public IActionResult Create()
        {
            ViewData["PurchaseOrderId"] = new SelectList(_context.PurchaseOrder, "PurchaseOrderId", "Number");
            ViewData["Number"] = _pos.GenerateGRNumber();
            return View();
        }

        // POST: GoodsReceive/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GoodsReceiveId,Number,Description,GoodsReceiveDate,PurchaseOrderId")] GoodsReceive goodsReceive)
        {
            if (ModelState.IsValid)
            {
                goodsReceive.GoodsReceiveId = Guid.NewGuid();
                _context.Add(goodsReceive);
                List<PurchaseOrderLine> purchLines = new List<PurchaseOrderLine>();
                purchLines = await _context.PurchaseOrderLine.Where(x => x.PurchaseOrderId.Equals(goodsReceive.PurchaseOrderId)).ToListAsync();
                foreach (var item in purchLines)
                {
                    GoodsReceiveLine line = new GoodsReceiveLine();
                    line.GoodsReceiveId = goodsReceive.GoodsReceiveId;
                    line.PurchaseOrderLineId = item.PurchaseOrderLineId;
                    line.ProductId = item.ProductId;
                    line.QtyPurchase = item.Quantity;
                    line.QtyReceive = 0;
                    List<GoodsReceiveLine> received = new List<GoodsReceiveLine>();
                    received = await _context.GoodsReceiveLine.Where(x => x.PurchaseOrderLineId.Equals(item.PurchaseOrderLineId)).ToListAsync();
                    line.QtyReceived = received.Sum(x => x.QtyReceive);
                    _context.Add(line);

                    InvenTran tran = new InvenTran();
                    tran.InvenTranId = Guid.NewGuid();
                    tran.Number = _pos.GenerateInvenTranNumber();
                    tran.ProductId = line.ProductId;
                    tran.TranSourceId = line.GoodsReceiveLineId;
                    tran.TranSourceNumber = goodsReceive.Number;
                    tran.TranSourceType = "GR";
                    tran.Quantity = line.QtyReceive * 1;
                    tran.InvenTranDate = DateTime.Now;
                    _context.Add(tran);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = goodsReceive.GoodsReceiveId });
            }
            ViewData["PurchaseOrderId"] = new SelectList(_context.PurchaseOrder, "PurchaseOrderId", "Number", goodsReceive.PurchaseOrderId);
            return View(goodsReceive);
        }

        // GET: GoodsReceive/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goodsReceive = await _context.GoodsReceive
                .Include(x => x.PurchaseOrder)
                .Where(x => x.GoodsReceiveId.Equals(id)).FirstOrDefaultAsync();
            if (goodsReceive == null)
            {
                return NotFound();
            }
            ViewData["PurchaseOrderId"] = new SelectList(_context.PurchaseOrder, "PurchaseOrderId", "Number", goodsReceive.PurchaseOrderId);
            return View(goodsReceive);
        }

        // POST: GoodsReceive/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("GoodsReceiveId,Number,Description,GoodsReceiveDate,PurchaseOrderId")] GoodsReceive goodsReceive)
        {
            if (id != goodsReceive.GoodsReceiveId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(goodsReceive);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoodsReceiveExists(goodsReceive.GoodsReceiveId))
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
            ViewData["PurchaseOrderId"] = new SelectList(_context.PurchaseOrder, "PurchaseOrderId", "Number", goodsReceive.PurchaseOrderId);
            return View(goodsReceive);
        }

        // GET: GoodsReceive/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goodsReceive = await _context.GoodsReceive
                .Include(g => g.PurchaseOrder)
                .FirstOrDefaultAsync(m => m.GoodsReceiveId == id);
            if (goodsReceive == null)
            {
                return NotFound();
            }

            return View(goodsReceive);
        }

        // POST: GoodsReceive/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var goodsReceive = await _context.GoodsReceive.FindAsync(id);

            List<GoodsReceiveLine> lines = new List<GoodsReceiveLine>();
            lines = await _context.GoodsReceiveLine.Where(x => x.GoodsReceiveId.Equals(id)).ToListAsync();

            List<InvenTran> trans = new List<InvenTran>();
            trans = await _context.InvenTran.Where(x => x.TranSourceNumber.Equals(goodsReceive.Number)).ToListAsync();

            _context.InvenTran.RemoveRange(trans);
            _context.GoodsReceiveLine.RemoveRange(lines);
            _context.GoodsReceive.Remove(goodsReceive);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoodsReceiveExists(Guid id)
        {
            return _context.GoodsReceive.Any(e => e.GoodsReceiveId == id);
        }
    }
}
