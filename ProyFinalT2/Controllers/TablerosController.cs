using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using ProyFinalT2.Entidades;
using ProyFinalT2.Models;
using ProyFinalT2.Servicios;

namespace ProyFinalT2.Controllers
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
        public async Task<ActionResult<List<TableroDTO>>> Get()
        {
            if (!int.TryParse(servicioUsuarios.ObtenerUsuarioId(), out var usuarioId))
            {
                return BadRequest("El ID de usuario no es válido.");
            }

            var tableros = await context.Tableros
                .Where(t => t.IdUsuarioPropietario == usuarioId ||
                            t.Tareas.Any(tarea => tarea.UsuarioCreacionId == usuarioId))
                .ProjectTo<TableroDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            return tableros;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tablero>> Get(int id)
        {
            if (!int.TryParse(servicioUsuarios.ObtenerUsuarioId(), out var usuarioId))
            {
                return BadRequest("El ID de usuario no es válido.");
            }

            var tablero = await context.Tableros
                .Include(t => t.Tareas)
                .FirstOrDefaultAsync(t => t.Id == id &&
                    (t.IdUsuarioPropietario == usuarioId ||
                     t.Tareas.Any(tarea => tarea.UsuarioCreacionId == usuarioId)));

            if (tablero is null)
            {
                return NotFound();
            }

            return tablero;
        }

        [HttpPost]
        public async Task<ActionResult<Tablero>> Post([FromBody] string nombre)
        {
            if (!int.TryParse(servicioUsuarios.ObtenerUsuarioId(), out var usuarioId))
            {
                return BadRequest("El ID de usuario no es válido.");
            }

            var tablero = new Tablero
            {
                Nombre = nombre,
                IdUsuarioPropietario = usuarioId
            };

            context.Add(tablero);
            await context.SaveChangesAsync();

            return tablero;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditarTablero(int id, [FromBody] TableroEditarDTO tableroEditarDTO)
        {
            if (!int.TryParse(servicioUsuarios.ObtenerUsuarioId(), out var usuarioId))
            {
                return BadRequest("El ID de usuario no es válido.");
            }

            var tablero = await context.Tableros.FirstOrDefaultAsync(t =>
                t.Id == id && t.IdUsuarioPropietario == usuarioId);

            if (tablero is null)
            {
                return NotFound();
            }

            tablero.Nombre = tableroEditarDTO.Nombre;
            tablero.Descripcion = tableroEditarDTO.Descripcion;

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (!int.TryParse(servicioUsuarios.ObtenerUsuarioId(), out var usuarioId))
            {
                return BadRequest("El ID de usuario no es válido.");
            }

            var tablero = await context.Tableros.FirstOrDefaultAsync(t =>
                t.Id == id && t.IdUsuarioPropietario == usuarioId);

            if (tablero is null)
            {
                return NotFound();
            }

            context.Remove(tablero);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

namespace ProyFinalT2.Models
{
    public class TableroEditarDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}
