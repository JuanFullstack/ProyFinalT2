﻿using System.ComponentModel.DataAnnotations;

namespace ProyFinalT2.Models
{
    public class TareaEditarDTO
    {
        [Required]
        [StringLength(250)]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
    }

}
