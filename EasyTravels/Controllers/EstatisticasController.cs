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
    //[Authorize]

    public class EstatisticasController : ControllerBase
    {
        private readonly EasyTravelsDbV2Context _context;

        public EstatisticasController(EasyTravelsDbV2Context context)
        {
            _context = context;
        }

        // GET: api/Estatisticas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Estatistica>>> GetEstatisticas()
        {
            return await _context.Estatisticas.ToListAsync();
        }

        // GET: api/Estatisticas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Estatistica>> GetEstatistica(int id)
        {
            var estatistica = await _context.Estatisticas.FindAsync(id);

            if (estatistica == null)
            {
                return NotFound();
            }

            return estatistica;
        }

        [HttpGet("EstatisticasComViagem")]
        public async Task<ActionResult<IEnumerable<Estatistica>>> GetEstatisticasComViagem()
        {
            var estatisticas = await _context.Estatisticas
                .Include(e => e.Viagem) // Inclui informações da viagem associada
                .ToListAsync();

            // Opcional: Retornar um DTO com os dados formatados, se necessário
            var estatisticasComViagens = estatisticas.Select(e => new
            {
                e.Id,
                e.ViagemId,
                Viagem = e.Viagem != null ? new
                {
                    e.Viagem.Destino,
                    e.Viagem.DataInicio,
                    e.Viagem.DataFim,
                    e.Viagem.OrganizadorId,
                    e.Viagem.CustoTotal
                } : null,
                e.Participantes,
                e.Despesas,
                e.LocaisMaisVisitados,
                e.Tipo,
                e.DuracaoDias,
                e.MediaCustoPorParticipante
            });

            return Ok(estatisticasComViagens);
        }

        // PUT: api/Estatisticas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstatistica(int id, Estatistica estatistica)
        {
            if (id != estatistica.Id)
            {
                return BadRequest();
            }

            _context.Entry(estatistica).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstatisticaExists(id))
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

        // POST: api/Estatisticas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Estatistica>> PostEstatistica(Estatistica estatistica)
        {
            _context.Estatisticas.Add(estatistica);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EstatisticaExists(estatistica.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEstatistica", new { id = estatistica.Id }, estatistica);
        }

        // DELETE: api/Estatisticas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstatistica(int id)
        {
            var estatistica = await _context.Estatisticas.FindAsync(id);
            if (estatistica == null)
            {
                return NotFound();
            }

            _context.Estatisticas.Remove(estatistica);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EstatisticaExists(int id)
        {
            return _context.Estatisticas.Any(e => e.Id == id);
        }
    }
}
