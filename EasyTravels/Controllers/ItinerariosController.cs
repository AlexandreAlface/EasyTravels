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

    public class ItinerariosController : ControllerBase
    {
        private readonly EasyTravelsDbV2Context _context;

        public ItinerariosController(EasyTravelsDbV2Context context)
        {
            _context = context;
        }

        // GET: api/Itinerarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Itinerario>>> GetItinerarios()
        {
            return await _context.Itinerarios.ToListAsync();
        }

        // GET: api/Itinerarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Itinerario>> GetItinerario(int id)
        {
            var itinerario = await _context.Itinerarios.FindAsync(id);

            if (itinerario == null)
            {
                return NotFound();
            }

            return itinerario;
        }

        // PUT: api/Itinerarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItinerario(int id, Itinerario itinerario)
        {
            if (id != itinerario.Id)
            {
                return BadRequest();
            }

            _context.Entry(itinerario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItinerarioExists(id))
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

        // POST: api/Itinerarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Itinerario>> PostItinerario(Itinerario itinerario)
        {
            _context.Itinerarios.Add(itinerario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItinerario", new { id = itinerario.Id }, itinerario);
        }

        // DELETE: api/Itinerarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItinerario(int id)
        {
            var itinerario = await _context.Itinerarios.FindAsync(id);
            if (itinerario == null)
            {
                return NotFound();
            }

            _context.Itinerarios.Remove(itinerario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItinerarioExists(int id)
        {
            return _context.Itinerarios.Any(e => e.Id == id);
        }


        [HttpGet("ItinerariosCompletos")]
        public async Task<ActionResult<IEnumerable<DetalhesCompletosDto>>> ItinerariosCompletos(
        string atividade = null,
        int? organizador = null,
        DateTime? dataInicio = null,
        DateTime? dataFim = null)
        {
            var query = _context.Itinerarios
                .Include(i => i.Viagem)
                .ThenInclude(v => v.Organizador) // Inclui informações do organizador
                .AsQueryable();

            // Filtrar por atividade
            if (!string.IsNullOrEmpty(atividade))
            {
                query = query.Where(i => i.Atividade.Contains(atividade));
            }

            // Filtrar por organizador
            if (organizador.HasValue)
            {
                query = query.Where(i => i.Viagem.OrganizadorId == organizador.Value);
            }

            // Filtrar por data de início
            if (dataInicio.HasValue)
            {
                query = query.Where(i => i.Viagem.DataInicio >= DateOnly.FromDateTime(dataInicio.Value));
            }

            // Filtrar por data de fim
            if (dataFim.HasValue)
            {
                query = query.Where(i => i.Viagem.DataFim <= DateOnly.FromDateTime(dataFim.Value));
            }

            // Executar a consulta e projetar os resultados
            var resultados = await query.Select(i => new
            {
                ItinerarioId = i.Id,
                i.Atividade,
                i.Descricao,
                ItinerarioData = i.Data,
                ViagemId = i.ViagemId,
                ViagemDestino = i.Viagem.Destino,
                ViagemDataInicio = i.Viagem.DataInicio,
                ViagemDataFim = i.Viagem.DataFim,
                OrganizadorId = i.Viagem.Organizador != null ? i.Viagem.Organizador.Id : (int?)null,
                OrganizadorNome = i.Viagem.Organizador != null ? i.Viagem.Organizador.Nome : null
            }).ToListAsync();

            return Ok(resultados);
        }

    }
}
