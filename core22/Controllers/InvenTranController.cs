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
    public class InvenTranController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvenTranController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InvenTran
        public async Task<IActionResult> Index()
        {
            return View(await _context.InvenTran.Include(x => x.Product).ToListAsync());
        }

        // GET: InvenTran/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invenTran = await _context.InvenTran
                .FirstOrDefaultAsync(m => m.InvenTranId == id);
            if (invenTran == null)
            {
                return NotFound();
            }

            return View(invenTran);
        }

        // GET: InvenTran/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InvenTran/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvenTranId,Number,Description,TranSourceNumber,TranSourceType,Quantity,InvenTranDate")] InvenTran invenTran)
        {
            if (ModelState.IsValid)
            {
                invenTran.InvenTranId = Guid.NewGuid();
                _context.Add(invenTran);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(invenTran);
        }

        // GET: InvenTran/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invenTran = await _context.InvenTran.FindAsync(id);
            if (invenTran == null)
            {
                return NotFound();
            }
            return View(invenTran);
        }

        // POST: InvenTran/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("InvenTranId,Number,Description,TranSourceNumber,TranSourceType,Quantity,InvenTranDate")] InvenTran invenTran)
        {
            if (id != invenTran.InvenTranId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invenTran);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvenTranExists(invenTran.InvenTranId))
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
            return View(invenTran);
        }

        // GET: InvenTran/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invenTran = await _context.InvenTran
                .FirstOrDefaultAsync(m => m.InvenTranId == id);
            if (invenTran == null)
            {
                return NotFound();
            }

            return View(invenTran);
        }

        // POST: InvenTran/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var invenTran = await _context.InvenTran.FindAsync(id);
            _context.InvenTran.Remove(invenTran);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvenTranExists(Guid id)
        {
            return _context.InvenTran.Any(e => e.InvenTranId == id);
        }
    }
}
