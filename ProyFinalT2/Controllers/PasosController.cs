using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProyFinalT2.Entidades;
using ProyFinalT2.Models;
using ProyFinalT2.Servicios;
using ProyFinalT2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProyFinalT2.Controllers
{
    [Route("api/tableros")]
    public class TablerosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IMapper mapper;

        public TablerosController(ApplicationDbContext context,
            IServicioUsuarios servicioUsuarios,
            IMapper mapper)
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
                .Where(t => t.UsuarioCreacionId == usuarioId)
                .OrderBy(t => t.Orden)
                .ProjectTo<TableroDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            return tableros;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tablero>> Get(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tablero = await context.Tableros
                .Include(t => t.Pasos.OrderBy(p => p.Orden))
                .FirstOrDefaultAsync(t => t.Id == id &&
            t.UsuarioCreacionId == usuarioId);

            if (tablero is null)
            {
                return NotFound();
            }

            return tablero;

        }

        [HttpPost]
        public async Task<ActionResult<Tablero>> Post([FromBody] string titulo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var existenTableros = await context.Tableros.AnyAsync(t => t.UsuarioCreacionId == usuarioId);

            var ordenMayor = 0;
            if (existenTableros)
            {
                ordenMayor = await context.Tableros.Where(t => t.UsuarioCreacionId == usuarioId)
                    .Select(t => t.Orden).MaxAsync();
            }

            var tablero = new Tablero
            {
                Titulo = titulo,
                UsuarioCreacionId = usuarioId,
                FechaCreacion = DateTime.UtcNow,
                Orden = ordenMayor + 1
            };

            context.Add(tablero);
            await context.SaveChangesAsync();

            return tablero;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditarTablero(int id, [FromBody] TableroEditarDTO tableroEditarDTO)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tablero = await context.Tableros.FirstOrDefaultAsync(t => t.Id == id &&
            t.UsuarioCreacionId == usuarioId);

            if (tablero is null)
            {
                return NotFound();
            }

            tablero.Titulo = tableroEditarDTO.Titulo;
            tablero.Descripcion = tableroEditarDTO.Descripcion;

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tablero = await context.Tableros.FirstOrDefaultAsync(t => t.Id == id &&
            t.UsuarioCreacionId == usuarioId);

            if (tablero is null)
            {
                return NotFound();
            }

            context.Remove(tablero);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("ordenar")]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tableros = await context.Tableros
                .Where(t => t.UsuarioCreacionId == usuarioId).ToListAsync();

            var tablerosId = tableros.Select(t => t.Id);

            var idsTablerosNoPertenecenAlUsuario = ids.Except(tablerosId).ToList();

            if (idsTablerosNoPertenecenAlUsuario.Any())
            {
                return Forbid();
            }

            var tablerosDiccionario = tableros.ToDictionary(x => x.Id);

            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var tablero = tablerosDiccionario[id];
                tablero.Orden = i + 1;
            }

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}