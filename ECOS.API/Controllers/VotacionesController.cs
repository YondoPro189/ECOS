using ECOS.API.Data;
using ECOS.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECOS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotacionesController : ControllerBase
    {
        private readonly EcosDbContext _context;

        public VotacionesController(EcosDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Votacion>>> GetVotaciones()
        {
            return await _context.Votaciones
                .Include(v => v.Juez)
                .Include(v => v.Equipo)
                .ToListAsync();
        }

        [HttpGet("Resultados")]
        public async Task<ActionResult<IEnumerable<ResultadoVotacion>>> GetResultados()
        {
            var resultados = await _context.Votaciones
                .Include(v => v.Equipo)
                .GroupBy(v => v.EquipoId)
                .Select(g => new ResultadoVotacion
                {
                    EquipoId = g.Key,
                    NombreEquipo = g.First().Equipo!.Nombre,
                    TotalVotos = g.Count()
                })
                .OrderByDescending(r => r.TotalVotos)
                .ToListAsync();

            return Ok(resultados);
        }

        [HttpPost("Votar")]
        public async Task<ActionResult<Votacion>> Votar([FromBody] VotarRequest request)
        {
            var juez = await _context.Jueces.FindAsync(request.JuezId);
            if (juez == null || !juez.Activo)
            {
                return BadRequest("Juez no encontrado o inactivo");
            }

            if (juez.YaVoto)
            {
                return BadRequest("Este juez ya votó anteriormente");
            }

            var votoExistente = await _context.Votaciones
                .AnyAsync(v => v.JuezId == request.JuezId);

            if (votoExistente)
            {
                return BadRequest("Ya existe un voto registrado para este juez");
            }

            var equipo = await _context.Equipos.FindAsync(request.EquipoId);
            if (equipo == null || !equipo.Activo)
            {
                return BadRequest("Equipo no encontrado o inactivo");
            }

            var votacion = new Votacion
            {
                JuezId = request.JuezId,
                EquipoId = request.EquipoId,
                FechaVoto = DateTime.Now
            };

            _context.Votaciones.Add(votacion);

            juez.YaVoto = true;

            await _context.SaveChangesAsync();

            await _context.Entry(votacion).Reference(v => v.Juez).LoadAsync();
            await _context.Entry(votacion).Reference(v => v.Equipo).LoadAsync();

            return CreatedAtAction(nameof(GetVotaciones), new { id = votacion.Id }, votacion);
        }

        [HttpGet("PuedeVotar/{juezId}")]
        public async Task<ActionResult<bool>> PuedeVotar(int juezId)
        {
            var juez = await _context.Jueces.FindAsync(juezId);
            if (juez == null || !juez.Activo)
            {
                return false;
            }

            return !juez.YaVoto;
        }

        [HttpDelete("ResetearVotos")]
        public async Task<IActionResult> ResetearVotos()
        {
            var votaciones = await _context.Votaciones.ToListAsync();
            _context.Votaciones.RemoveRange(votaciones);

            var jueces = await _context.Jueces.ToListAsync();
            foreach (var juez in jueces)
            {
                juez.YaVoto = false;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class VotarRequest
    {
        public int JuezId { get; set; }
        public int EquipoId { get; set; }
    }

    public class ResultadoVotacion
    {
        public int EquipoId { get; set; }
        public string NombreEquipo { get; set; } = string.Empty;
        public int TotalVotos { get; set; }
    }
}