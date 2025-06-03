using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyTravelsAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace EasyTravelsAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlojamentoesController : ControllerBase
    {
        private readonly EasyTravelsDbV2Context _context;

        public AlojamentoesController(EasyTravelsDbV2Context context)
        {
            _context = context;
        }

        // GET: api/Alojamentoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alojamento>>> GetAlojamentos()
        {
            return await _context.Alojamentos.ToListAsync();
        }

        // GET: api/Alojamentoes/5
        [HttpGet("{id}")]
        [AllowAnonymous]

        public async Task<ActionResult<Alojamento>> GetAlojamento(int id)
        {
            var alojamento = await _context.Alojamentos.FindAsync(id);

            if (alojamento == null)
            {
                return NotFound();
            }

            return alojamento;
        }

        // PUT: api/Alojamentoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlojamento(int id, Alojamento alojamento)
        {
            if (id != alojamento.Id)
            {
                return BadRequest();
            }

            _context.Entry(alojamento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlojamentoExists(id))
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

        // POST: api/Alojamentoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Alojamento>> PostAlojamento(Alojamento alojamento)
        {
            _context.Alojamentos.Add(alojamento);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AlojamentoExists(alojamento.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAlojamento", new { id = alojamento.Id }, alojamento);
        }

        // DELETE: api/Alojamentoes/5
        [HttpDelete("{id}")]
        [AllowAnonymous]

        public async Task<IActionResult> DeleteAlojamento(int id)
        {
            var alojamento = await _context.Alojamentos.FindAsync(id);
            if (alojamento == null)
            {
                return NotFound();
            }

            _context.Alojamentos.Remove(alojamento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AlojamentoExists(int id)
        {
            return _context.Alojamentos.Any(e => e.Id == id);
        }

        [HttpGet("GetAlojamentosByViagemId")]
        [AllowAnonymous]

        public IActionResult GetAlojamentosByViagemId(int viagemId)
        {
            var alojamentos = _context.Alojamentos.Where(t => t.ViagemId == viagemId).ToList();
            return Ok(alojamentos);
        }
    }
}
