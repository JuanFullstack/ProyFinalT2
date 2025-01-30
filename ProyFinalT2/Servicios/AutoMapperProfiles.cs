using AutoMapper;
using ProyFinalT2.Entidades;
using ProyFinalT2.Models;

namespace ProyFinalT2.Servicios
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Tarea, TareaDTO>();
        }
    }
}
