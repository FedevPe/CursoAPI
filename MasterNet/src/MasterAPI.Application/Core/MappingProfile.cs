using AutoMapper;
using MasterAPI.Application.Calificaciones.CalificacionesGet;
using MasterAPI.Application.Cursos.CursoGet;
using MasterAPI.Application.Imagenes.ImagenGet;
using MasterAPI.Application.Instructores.InstructoresGet;
using MasterAPI.Application.Precios.PreciosGet;
using MasterAPI.Domain.Models;

namespace MasterAPI.Application.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Curso, CursoDTO>();
            CreateMap<Imagen, ImagenDTO>();
            CreateMap<Precio, PrecioDTO>();
            CreateMap<Instructor, InstructorDTO>();


            //Personalizacion de mapeo, se utiliza cuando hay propiedades en el objeto de origen y destino que no tienen el mismo nombre
            //pero son equivalentes.
            CreateMap<Calificacion, CalificacionDTO>()
                .ForMember(dest => dest.NombreCurso, src => src.MapFrom(doc => doc.Curso!.Titulo));

            
        }
    }
}