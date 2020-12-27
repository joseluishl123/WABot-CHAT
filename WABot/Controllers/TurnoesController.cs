using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WABot.ModeloTurno;

namespace WABot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnoesController : ControllerBase
    {
        private readonly DB_Turnos_WhatsappContext _context;

        public TurnoesController(DB_Turnos_WhatsappContext context)
        {
            _context = context;
        }

        // GET: api/Turnoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Turno>>> GetTurnos()
        {
            return await _context.Turnos
                .Include(s => s.TurnIdPacienteNavigation)
                .Where(s => s.TurnEstado == 1)
                .ToListAsync();
        }

        // GET: api/Turnoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Turno>> GetTurno(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);

            if (turno == null)
            {
                return NotFound();
            }

            return turno;
        }

        // PUT: api/Turnoes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTurno(int id, Turno turno)
        {
            if (id != turno.TurnId)
            {
                return BadRequest();
            }

            _context.Entry(turno).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TurnoExists(id))
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

        // POST: api/Turnoes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Turno>> PostTurno(Turno turno)
        {
            if (turno.TurnIdPacienteNavigation != null)
            {
                var turnoVacio = new Turno();
                var turnoActivo = await _context.Turnos.Where(
                    opc => opc.TurnIdPacienteNavigation.PerTelefono == turno.TurnIdPacienteNavigation.PerTelefono && opc.TurnEstado == 1).ToListAsync();
                if (turnoActivo != null)
                {
                    turnoActivo = turnoActivo.OrderByDescending(s => s.TurnCodigo).ToList();
                    //return Ok(turnoActivo);
                    return CreatedAtAction("GetTurno", new { id = turno.TurnId }, turnoActivo);
                }
            }
            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();
            var turn = await _context.Turnos.Include(s => s.TurnEstadoNavigation).Include(s => s.TurnIdPacienteNavigation).FirstOrDefaultAsync(s => s.TurnId == turno.TurnId);
            return CreatedAtAction("GetTurno", new { id = turno.TurnId }, turn);
        }

        // DELETE: api/Turnoes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Turno>> DeleteTurno(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null)
            {
                return NotFound();
            }
            _context.Turnos.Remove(turno);
            await _context.SaveChangesAsync();
            return turno;
        }

        private bool TurnoExists(int id)
        {
            return _context.Turnos.Any(e => e.TurnId == id);
        }
    }
}
