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
    public class SalesOrderLineController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly Services.POS.IRepository _pos;

        public SalesOrderLineController(ApplicationDbContext context, IHttpContextAccessor httpContext, Services.POS.IRepository pos)
        {
            _context = context;
            _httpContext = httpContext;
            _pos = pos;
        }

        // GET: api/SalesOrderLine
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderLine>>> GetSalesOrderLine()
        {
            return await _context.SalesOrderLine.ToListAsync();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLines()
        {
            List<SalesOrderLine> lines = new List<SalesOrderLine>();
            try
            {
                var paramGuidString = _httpContext.HttpContext.Request.Query["salesOrderId"].ToString();
                if (!string.IsNullOrEmpty(paramGuidString))
                {
                    Guid salesOrderId = new Guid(paramGuidString);
                    lines = await _context.SalesOrderLine.Include(x => x.Product).Where(x => x.SalesOrderId.Equals(salesOrderId)).ToListAsync();
                }
                return Ok(new { lines });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = ex.Message });
            }

        }

        // GET: api/SalesOrderLine/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderLine>> GetSalesOrderLine(Guid id)
        {
            var salesOrderLine = await _context.SalesOrderLine.FindAsync(id);

            if (salesOrderLine == null)
            {
                return NotFound();
            }

            return salesOrderLine;
        }

        // PUT: api/SalesOrderLine/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderLine(Guid id, SalesOrderLine salesOrderLine)
        {
            if (id != salesOrderLine.SalesOrderLineId)
            {
                return BadRequest();
            }

            _context.Entry(salesOrderLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesOrderLineExists(id))
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

        // POST: api/SalesOrderLine
        [HttpPost]
        public async Task<ActionResult<SalesOrderLine>> PostSalesOrderLine(SalesOrderLine salesOrderLine)
        {
            _context.SalesOrderLine.Add(salesOrderLine);

            SalesOrder salesOrder = _context.SalesOrder.Where(x => x.SalesOrderId.Equals(salesOrderLine.SalesOrderId)).FirstOrDefault();
            InvenTran tran = new InvenTran();
            tran.InvenTranId = Guid.NewGuid();
            tran.Number = _pos.GenerateInvenTranNumber();
            tran.ProductId = salesOrderLine.ProductId;
            tran.TranSourceId = salesOrderLine.SalesOrderLineId;
            tran.TranSourceNumber = salesOrder != null ? salesOrder.Number : "-";
            tran.TranSourceType = "SO";
            tran.Quantity = salesOrderLine.Quantity * -1; //minus for inventory deduction
            tran.InvenTranDate = DateTime.Now;
            _context.InvenTran.Add(tran);

            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetSalesOrderLine", new { id = salesOrderLine.SalesOrderLineId }, salesOrderLine);
        }

        // DELETE: api/SalesOrderLine/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SalesOrderLine>> DeleteSalesOrderLine(Guid id)
        {
            var salesOrderLine = await _context.SalesOrderLine.FindAsync(id);
            if (salesOrderLine == null)
            {
                return NotFound();
            }
            var tran = await _context.InvenTran.Where(x => x.TranSourceId.Equals(id)).FirstOrDefaultAsync();
            _context.InvenTran.Remove(tran);
            _context.SalesOrderLine.Remove(salesOrderLine);

            await _context.SaveChangesAsync();

            return salesOrderLine;
        }

        private bool SalesOrderLineExists(Guid id)
        {
            return _context.SalesOrderLine.Any(e => e.SalesOrderLineId == id);
        }
    }
}
