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

    public class ReservasController : ControllerBase
    {
        private readonly EasyTravelsDbV2Context _context;

        public ReservasController(EasyTravelsDbV2Context context)
        {
            _context = context;
        }

        // GET: api/Reservas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            return await _context.Reservas.ToListAsync();
        }

        // GET: api/Reservas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReserva(int id)
        {
            var reserva = await _context.Reservas
              .Include(r => r.Viagem)
                  .ThenInclude(v => v.Itinerarios) 
              .Include(r => r.Viagem.Alojamentos) 
              .Include(r => r.Viagem.Transportes) 
              .Include(r => r.Viagem.Organizador)
              .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            var reservaDto = new ReservaDTO
            {
                Id = reserva.Id,
                Status = reserva.Status,
                Tipo = reserva.Tipo,
                Viagem = new ViagemDTO
                {
                    Id = reserva.Viagem.Id,
                    Destino = reserva.Viagem.Destino,
                    DataInicio = reserva.Viagem.DataInicio,
                    DataFim = reserva.Viagem.DataFim,
                    Itinerarios = reserva.Viagem.Itinerarios.Select(i => new ItinerarioDTO
                    {
                        Atividade = i.Atividade,
                        Data = i.Data,
                        Descricao = i.Descricao
                    }).ToList(),
                    Alojamentos = reserva.Viagem.Alojamentos.Select(a => new AlojamentoDTO
                    {
                        Nome = a.Nome,
                        Custo = a.Custo,
                        Endereco = a.Endereco
                    }).ToList(),
                    Transportes = reserva.Viagem.Transportes.Select(t => new TransporteDTO
                    {
                        Tipo = t.Tipo,
                        Custo = t.Custo,
                        Detalhes = t.Detalhes
                    }).ToList(),
                    Organizador = new UtilizadorDTO
                    {
                        Nome = reserva.Viagem.Organizador.Nome
                    }
                }
            };

            return Ok(reservaDto);
        }



        // PUT: api/Reservas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReserva(int id, Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return BadRequest();
            }

            var existingReserva = await _context.Reservas
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingReserva == null)
            {
                return NotFound();
            }

            // Atualizar apenas os campos necessários
            existingReserva.Status = reserva.Status;
            existingReserva.Tipo = reserva.Tipo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(id))
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


        // POST: api/Reservas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Reserva>> PostReserva(Reserva reserva)
        {
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReserva", new { id = reserva.Id }, reserva);
        }

        // DELETE: api/Reservas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }

        [HttpGet("ByViajante/{userId}")]
        public IActionResult GetReservationsWithDetails(int userId)
        {
            var reservations = _context.Reservas
               .Include(r => r.Viagem)
               .ThenInclude(v => v.Itinerarios)
               .Include(r => r.Viagem.Organizador)
               .Where(r => r.ViajanteId == userId)
               .Select(r => new Reserva
               {
                   Id = r.Id,
                   Tipo = r.Tipo,
                   Status = r.Status,
                   Viagem = new Viagem
                   {
                       Destino = r.Viagem.Destino,
                       DataInicio = r.Viagem.DataInicio,
                       DataFim = r.Viagem.DataFim,

                       Organizador = new Utilizador
                       {
                           Nome = r.Viagem.Organizador.Nome
                       },
                       Itinerarios = r.Viagem.Itinerarios.Select(i => new Itinerario
                       {
                           Atividade = i.Atividade,
                           Data = i.Data,
                           Descricao = i.Descricao
                       }).ToList()
                   }
               })
               .ToList();

            return Ok(reservations);
        }

        [HttpGet("ByOrganizador/{userId}")]
        public IActionResult GetReservationsByOrganizer(int userId)
        {
            // Obter reservas com as relações necessárias
            var reservations = _context.Reservas
                .Include(r => r.Viagem) // Inclui a viagem associada à reserva
                .ThenInclude(v => v.Itinerarios) // Inclui os itinerários da viagem
                .Include(r => r.Viagem.Organizador) // Inclui o organizador da viagem
                .Where(r => r.Viagem.OrganizadorId == userId) // Filtra por organizador
                .Select(r => new Reserva
                {
                    Id = r.Id,
                    Tipo = r.Tipo,
                    Status = r.Status,
                    Viagem = new Viagem
                    {
                        Destino = r.Viagem.Destino,
                        DataInicio = r.Viagem.DataInicio,
                        DataFim = r.Viagem.DataFim,
                        Organizador = new Utilizador
                        {
                            Nome = r.Viagem.Organizador.Nome
                        },
                        Itinerarios = r.Viagem.Itinerarios.Select(i => new Itinerario
                        {
                            Atividade = i.Atividade,
                            Data = i.Data,
                            Descricao = i.Descricao
                        }).ToList()
                    }
                })
                .ToList();

            return Ok(reservations);
        }

        [HttpGet("CompradoresPorViagem")]
        public async Task<ActionResult<IEnumerable<object>>> GetCompradoresPorViagem()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Viagem)
                .Include(r => r.Viajante)
                .GroupBy(r => r.Viagem.Destino)
                .Select(group => new
                {
                    Viagem = group.Key,
                    Compradores = group
                        .Select(r => r.Viajante.Nome)
                        .Distinct()
                        .ToList()
                })
                .ToListAsync();

            return Ok(reservas);
        }


    }
}
