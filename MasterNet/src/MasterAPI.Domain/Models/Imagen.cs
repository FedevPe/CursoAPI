namespace MasterAPI.Domain.Models
{
    public class Imagen : BaseEntity
    {
        public string? Url { get; set; }
               
        public Guid? CursoId { get; set; }
        public Curso? Curso { get; set; }
        public string? PublicId { get; set; }
    }
}