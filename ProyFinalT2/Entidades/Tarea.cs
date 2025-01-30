using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ProyFinalT2.Entidades
{
    public class Tarea
    {
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string UsuarioCreacionId { get; set; }
        public IdentityUser UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<Paso> Pasos { get; set; }
        public int Orden { get; set; }
    }
}
