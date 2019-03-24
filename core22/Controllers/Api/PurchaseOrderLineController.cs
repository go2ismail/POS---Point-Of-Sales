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
    public class PurchaseOrderLineController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public PurchaseOrderLineController(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        // GET: api/PurchaseOrderLine
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrderLine>>> GetPurchaseOrderLine()
        {
            return await _context.PurchaseOrderLine.ToListAsync();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLines()
        {            
            List<PurchaseOrderLine> lines = new List<PurchaseOrderLine>();
            try
            {
                var paramGuidString = _httpContext.HttpContext.Request.Query["purchaseOrderId"].ToString();
                Guid purchaseOrderId = new Guid(paramGuidString);
                lines = await _context.PurchaseOrderLine.Include(x => x.Product).Where(x => x.PurchaseOrderId.Equals(purchaseOrderId)).ToListAsync();
                return Ok(new { lines });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = ex.Message });
            }

        }

        // GET: api/PurchaseOrderLine/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrderLine>> GetPurchaseOrderLine(Guid id)
        {
            var purchaseOrderLine = await _context.PurchaseOrderLine.FindAsync(id);

            if (purchaseOrderLine == null)
            {
                return NotFound();
            }

            return purchaseOrderLine;
        }

        // PUT: api/PurchaseOrderLine/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrderLine(Guid id, PurchaseOrderLine purchaseOrderLine)
        {
            if (id != purchaseOrderLine.PurchaseOrderLineId)
            {
                return BadRequest();
            }

            _context.Entry(purchaseOrderLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderLineExists(id))
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

        // POST: api/PurchaseOrderLine
        [HttpPost]
        public async Task<ActionResult<PurchaseOrderLine>> PostPurchaseOrderLine(PurchaseOrderLine purchaseOrderLine)
        {
            _context.PurchaseOrderLine.Add(purchaseOrderLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchaseOrderLine", new { id = purchaseOrderLine.PurchaseOrderLineId }, purchaseOrderLine);
        }

        // DELETE: api/PurchaseOrderLine/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PurchaseOrderLine>> DeletePurchaseOrderLine(Guid id)
        {
            var purchaseOrderLine = await _context.PurchaseOrderLine.FindAsync(id);
            if (purchaseOrderLine == null)
            {
                return NotFound();
            }

            _context.PurchaseOrderLine.Remove(purchaseOrderLine);
            await _context.SaveChangesAsync();

            return purchaseOrderLine;
        }

        private bool PurchaseOrderLineExists(Guid id)
        {
            return _context.PurchaseOrderLine.Any(e => e.PurchaseOrderLineId == id);
        }
    }
}
