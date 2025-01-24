using System.ComponentModel.DataAnnotations;

namespace ProyFinalT2.Models
{
    //RegistroViewModel esta clase es un modelo que se utiliza para la vista de registro
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
