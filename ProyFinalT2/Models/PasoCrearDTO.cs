using System.ComponentModel.DataAnnotations;

namespace ProyFinalT2.Models
{
    public class PasoCrearDTO
    {
        [Required]
        public string Descripcion { get; set; }
        public bool Realizado { get; set; }

    }
}
