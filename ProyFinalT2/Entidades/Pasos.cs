using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProyFinalT2.Entidades
{
    public class Paso
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Titulo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Descripcion { get; set; }

        [Required]
        public int Estado { get; set; } // Por ejemplo: 1 = "Pendiente", 2 = "En Proceso", 3 = "Finalizada"
        public string Color { get; set; } // Color asociado a la tarea, ejemplo: "#FF5733"

        // Relación con Tablero
        public int IdTablero { get; set; }
        public Tablero Tablero { get; set; }
        public int Orden { get; set; }

        // Relación con Usuario asignado
        public string UsuarioCreacionId { get; set; }
        public IdentityUser UsuarioCreacion { get; set; }
        
    }
}
