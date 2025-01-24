
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using ProyFinalT2.Entidades;
using ProyFinalT2.Servicios;
using ProyFinalT2;

namespace ManejosTareas.Seguridad.JC.Controllers
{
    [Route("api/tableros")]
    public class TablerosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IMapper mapper;

        public TablerosController(ApplicationDbContext context, IServicioUsuarios servicioUsuarios, IMapper mapper)
        {
            this.context = context;
            this.servicioUsuarios = servicioUsuarios;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tablero>>> Get()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tableros = await context.Tableros
                .Where(t => t.IdUsuarioPropietario == int.Parse(usuarioId)) // Se asegura que coincidan los tipos
                .Include(t => t.Tareas) // Incluye las tareas relacionadas
                .ToListAsync();

            return tableros;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tablero>> Get(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tablero = await context.Tableros
                .Include(t => t.Tareas) // Incluye las tareas relacionadas
                .FirstOrDefaultAsync(t => t.Id == id && t.IdUsuarioPropietario == int.Parse(usuarioId)); // Comparación corregida

            if (tablero == null)
            {
                return NotFound();
            }

            return tablero;
        }

        [HttpPost]
        public async Task<ActionResult<Tablero>> Post([FromBody] Tablero tablero)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            tablero.IdUsuarioPropietario = int.Parse(usuarioId);
            context.Add(tablero);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = tablero.Id }, tablero);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] Tablero tableroActualizado)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tablero = await context.Tableros.FirstOrDefaultAsync(t => t.Id == id && t.IdUsuarioPropietario == int.Parse(usuarioId));
            if (tablero == null)
            {
                return NotFound();
            }

            tablero.Nombre = tableroActualizado.Nombre;
            tablero.Descripcion = tableroActualizado.Descripcion;

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tablero = await context.Tableros.FirstOrDefaultAsync(t => t.Id == id && t.IdUsuarioPropietario == int.Parse(usuarioId));
            if (tablero == null)
            {
                return NotFound();
            }

            context.Remove(tablero);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
