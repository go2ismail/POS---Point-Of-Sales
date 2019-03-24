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
    public class GoodsReceiveLineController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public GoodsReceiveLineController(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        // GET: api/GoodsReceiveLine
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoodsReceiveLine>>> GetGoodsReceiveLine()
        {
            return await _context.GoodsReceiveLine.ToListAsync();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLines()
        {
            List<GoodsReceiveLine> lines = new List<GoodsReceiveLine>();
            try
            {
                var paramGuidString = _httpContext.HttpContext.Request.Query["goodsReceiveId"].ToString();
                Guid goodsReceiveId = new Guid(paramGuidString);
                lines = await _context.GoodsReceiveLine.Include(x => x.Product).Where(x => x.GoodsReceiveId.Equals(goodsReceiveId)).ToListAsync();
                return Ok(new { lines });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = ex.Message });
            }

        }

        // GET: api/GoodsReceiveLine/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GoodsReceiveLine>> GetGoodsReceiveLine(Guid id)
        {
            var goodsReceiveLine = await _context.GoodsReceiveLine
                                        .Include(x => x.Product)
                                        .Where(x => x.GoodsReceiveLineId.Equals(id)).FirstOrDefaultAsync();

            if (goodsReceiveLine == null)
            {
                return NotFound();
            }

            return goodsReceiveLine;
        }

        // PUT: api/GoodsReceiveLine/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoodsReceiveLine(Guid id, GoodsReceiveLine goodsReceiveLine)
        {
            if (id != goodsReceiveLine.GoodsReceiveLineId)
            {
                return BadRequest();
            }

            GoodsReceiveLine update = new GoodsReceiveLine();
            update = await _context.GoodsReceiveLine.FindAsync(id);

            InvenTran tran = new InvenTran();
            tran = await _context.InvenTran.Where(x => x.TranSourceId.Equals(id)).FirstOrDefaultAsync();
            if (update != null && tran != null)
            {
                update.QtyReceive = goodsReceiveLine.QtyReceive;
                tran.Quantity = update.QtyReceive * 1;
                _context.Update(tran);
                _context.Update(update);
                await _context.SaveChangesAsync();
            }            

            return Ok(new { data = update });
        }

        // POST: api/GoodsReceiveLine
        [HttpPost]
        public async Task<ActionResult<GoodsReceiveLine>> PostGoodsReceiveLine(GoodsReceiveLine goodsReceiveLine)
        {
            _context.GoodsReceiveLine.Add(goodsReceiveLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGoodsReceiveLine", new { id = goodsReceiveLine.GoodsReceiveLineId }, goodsReceiveLine);
        }

        // DELETE: api/GoodsReceiveLine/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GoodsReceiveLine>> DeleteGoodsReceiveLine(Guid id)
        {
            var goodsReceiveLine = await _context.GoodsReceiveLine.FindAsync(id);
            if (goodsReceiveLine == null)
            {
                return NotFound();
            }

            _context.GoodsReceiveLine.Remove(goodsReceiveLine);
            await _context.SaveChangesAsync();

            return goodsReceiveLine;
        }

        private bool GoodsReceiveLineExists(Guid id)
        {
            return _context.GoodsReceiveLine.Any(e => e.GoodsReceiveLineId == id);
        }
    }
}
