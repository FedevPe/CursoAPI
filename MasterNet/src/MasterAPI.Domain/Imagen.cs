namespace MasterAPI.Domain
{
    public class Imagen : BaseEntity
    {
        public string? Url { get; set; }
               
        public Guid? CursoId { get; set; }
        public Curso? Curso { get; set; }
    }
}