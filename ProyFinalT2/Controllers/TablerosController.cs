//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using AutoMapper;
//using AutoMapper.QueryableExtensions;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Linq;
//using ProyFinalT2.Entidades;
//using ProyFinalT2.Models;
//using ProyFinalT2.Servicios;

//namespace ProyFinalT2.Controllers
//{
//    [Route("api/tableros")]
//    public class TablerosController : ControllerBase
//    {
//        private readonly ApplicationDbContext context;
//        private readonly IServicioUsuarios servicioUsuarios;
//        private readonly IMapper mapper;

//        public TablerosController(ApplicationDbContext context, IServicioUsuarios servicioUsuarios, IMapper mapper)
//        {
//            this.context = context;
//            this.servicioUsuarios = servicioUsuarios;
//            this.mapper = mapper;
//        }

//        [HttpGet]
//        public async Task<ActionResult<List<TableroDTO>>> Get()
//        {
//            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
//            var tableros = await context.Tableros
//                .Where(t => t.IdUsuarioPropietario == usuarioId)
//                .OrderBy(t => t.Id)
//                .ProjectTo<TableroDTO>(mapper.ConfigurationProvider)
//                .ToListAsync();

//            return tableros;
//        }

//        [HttpGet("{id:int}")]
//        public async Task<ActionResult<Tablero>> Get(int id)
//        {
//            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

//            var tablero = await context.Tableros.FirstOrDefaultAsync(t => t.Id == id &&
//            t.IdUsuarioPropietario == usuarioId);

//            if (tablero is null)
//            {
//                return NotFound();
//            }

//            return tablero;
//        }

//        [HttpPost]
//        public async Task<ActionResult<Tablero>> Post([FromBody] string nombre)
//        {
//            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

//            var tablero = new Tablero
//            {
//                Nombre = nombre,
//                IdUsuarioPropietario = usuarioId
//            };

//            context.Add(tablero);
//            await context.SaveChangesAsync();

//            return tablero;
//        }

//        [HttpPut("{id:int}")]
//        public async Task<IActionResult> EditarTablero(int id, [FromBody] TableroEditarDTO tableroEditarDTO)
//        {
//            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

//            var tablero = await context.Tableros.FirstOrDefaultAsync(t => t.Id == id &&
//            t.IdUsuarioPropietario == usuarioId);

//            if (tablero is null)
//            {
//                return NotFound();
//            }

//            tablero.Nombre = tableroEditarDTO.Nombre;

//            await context.SaveChangesAsync();

//            return Ok();
//        }

//        [HttpDelete("{id:int}")]
//        public async Task<ActionResult> Delete(int id)
//        {
//            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

//            var tablero = await context.Tableros.FirstOrDefaultAsync(t => t.Id == id &&
//            t.IdUsuarioPropietario == usuarioId);

//            if (tablero is null)
//            {
//                return NotFound();
//            }

//            context.Remove(tablero);
//            await context.SaveChangesAsync();
//            return Ok();
//        }
//    }
//}
