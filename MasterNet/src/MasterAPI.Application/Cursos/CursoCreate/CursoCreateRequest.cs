using Microsoft.AspNetCore.Http;

namespace MasterAPI.Application.Cursos.CursoCreate
{
    //Esta clase la estructura de datos que envia el cliente para crear un nuevo curso.
    public class CursoCreateRequest
    {
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        //IFormFile es un tipo de dato archivo que se envia desde un formulario web.
        public IFormFile? Foto { get; set; }
        public Guid? InstructorId { get; set; }
        public Guid? PrecioId { get; set; }
    }
}