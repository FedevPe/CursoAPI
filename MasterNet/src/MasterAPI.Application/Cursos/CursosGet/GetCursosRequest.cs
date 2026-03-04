using MasterAPI.Application.Core;

namespace MasterAPI.Application.Cursos.CursosGet
{
    //Esta clase requesenta el request del cliente, oses los parametros que envia el cliente al servidor
    //para filtrar y obtener los registros de los cursos.
    public class GetCursosRequest : PagingParams
    {
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
    }
}