using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using core22.Data;
using core22.Models.POS.Models;

namespace core22.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PurchaseOrder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrder()
        {
            return await _context.PurchaseOrder.ToListAsync();
        }

        // GET: api/PurchaseOrder/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrder(Guid id)
        {
            var purchaseOrder = await _context.PurchaseOrder.FindAsync(id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return purchaseOrder;
        }

        // PUT: api/PurchaseOrder/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrder(Guid id, PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.PurchaseOrderId)
            {
                return BadRequest();
            }

            _context.Entry(purchaseOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PurchaseOrder
        [HttpPost]
        public async Task<ActionResult<PurchaseOrder>> PostPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            _context.PurchaseOrder.Add(purchaseOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchaseOrder", new { id = purchaseOrder.PurchaseOrderId }, purchaseOrder);
        }

        // DELETE: api/PurchaseOrder/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PurchaseOrder>> DeletePurchaseOrder(Guid id)
        {
            var purchaseOrder = await _context.PurchaseOrder.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            _context.PurchaseOrder.Remove(purchaseOrder);
            await _context.SaveChangesAsync();

            return purchaseOrder;
        }

        private bool PurchaseOrderExists(Guid id)
        {
            return _context.PurchaseOrder.Any(e => e.PurchaseOrderId == id);
        }
    }
}
