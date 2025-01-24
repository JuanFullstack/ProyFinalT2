using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProyFinalT2.Entidades;
using ProyFinalT2.Servicios;
using ProyFinalT2.Models;


namespace ProyFinalT2.Controllers
{
    [Route("api/tableros")]
    public class TablerosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IMapper mapper;

        public TablerosController(ApplicationDbContext context,  IServicioUsuarios servicioUsuarios, IMapper mapper)
        {
            this.context = context;
             this.servicioUsuarios = servicioUsuarios;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TableroDTO>>> Get()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tableros = await context.Tableros
                .Where(t => t.UsuarioCreacionId == usuarioId ||
                            t.Tareas.Any(tarea => tarea.IdUsuarioAsignado == usuarioId))
                .ProjectTo<TableroDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            return tableros;
        }

        [HttpPost]
        public async Task<ActionResult<Tablero>> Post([FromBody] TableroCrearDTO tableroDTO)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tablero = mapper.Map<Tablero>(tableroDTO);
            tablero.UsuarioCreacionId = usuarioId;

            context.Add(tablero);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = tablero.Id }, tablero);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] TableroCrearDTO tableroDTO)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tablero = await context.Tableros.FirstOrDefaultAsync(t => t.Id == id && t.IdUsuarioPropietario == usuarioId);

            if (tablero == null)
            {
                return NotFound();
            }

            tablero.Nombre = tableroDTO.Nombre;
            tablero.Descripcion = tableroDTO.Descripcion;

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tablero = await context.Tableros.FirstOrDefaultAsync(t => t.Id == id && t.IdUsuarioPropietario == usuarioId);

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
