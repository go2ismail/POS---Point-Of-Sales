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
    public class GoodsReceiveController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GoodsReceiveController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GoodsReceive
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoodsReceive>>> GetGoodsReceive()
        {
            return await _context.GoodsReceive.ToListAsync();
        }

        // GET: api/GoodsReceive/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GoodsReceive>> GetGoodsReceive(Guid id)
        {
            var goodsReceive = await _context.GoodsReceive.FindAsync(id);

            if (goodsReceive == null)
            {
                return NotFound();
            }

            return goodsReceive;
        }

        // PUT: api/GoodsReceive/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoodsReceive(Guid id, GoodsReceive goodsReceive)
        {
            if (id != goodsReceive.GoodsReceiveId)
            {
                return BadRequest();
            }

            _context.Entry(goodsReceive).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoodsReceiveExists(id))
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

        // POST: api/GoodsReceive
        [HttpPost]
        public async Task<ActionResult<GoodsReceive>> PostGoodsReceive(GoodsReceive goodsReceive)
        {
            _context.GoodsReceive.Add(goodsReceive);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGoodsReceive", new { id = goodsReceive.GoodsReceiveId }, goodsReceive);
        }

        // DELETE: api/GoodsReceive/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GoodsReceive>> DeleteGoodsReceive(Guid id)
        {
            var goodsReceive = await _context.GoodsReceive.FindAsync(id);
            if (goodsReceive == null)
            {
                return NotFound();
            }

            _context.GoodsReceive.Remove(goodsReceive);
            await _context.SaveChangesAsync();

            return goodsReceive;
        }

        private bool GoodsReceiveExists(Guid id)
        {
            return _context.GoodsReceive.Any(e => e.GoodsReceiveId == id);
        }
    }
}
