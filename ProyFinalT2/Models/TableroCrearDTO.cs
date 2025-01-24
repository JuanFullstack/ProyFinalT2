using System.ComponentModel.DataAnnotations;

namespace ProyFinalT2.Models
{
    public class TableroCrearDTO
    {
        [Required]
        [StringLength(250)]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}
