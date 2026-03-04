using MasterAPI.Application.Core;

namespace MasterAPI.Application.Calificaciones.GetCalificaciones;

public class GetCalificacionesRequest : PagingParams
{
    public string? Alumno {get;set;}
    public Guid? CursoId {get;set;}

}