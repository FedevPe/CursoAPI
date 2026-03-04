using MasterAPI.Application.Core;

namespace MasterAPI.Application.Instructores.InstructoresGet
{
    public class GetInstructoresRequest : PagingParams
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
    }
}