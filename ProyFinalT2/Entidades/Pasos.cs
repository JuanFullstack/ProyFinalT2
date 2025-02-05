using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProyFinalT2.Entidades
{
    public class Paso
    {
        public Guid Id { get; set; }
        public int TareaId { get; set; }
        // un paso se relaciona una tarea 
        public Tarea Tarea { get; set; }
        public string Descripcion { get; set; }
        public bool Realizado { get; set; }
        public int Orden { get; set; }

    }
}
