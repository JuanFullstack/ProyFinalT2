using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ProyFinalT2.Entidades
{
    public class Tablero
    {
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdUsuarioPropietario { get; set; }
        public List<Tarea> Tareas { get; set; } = new List<Tarea>();
        public int Orden { get; set; }
    }
}
