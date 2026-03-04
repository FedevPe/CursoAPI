namespace MasterAPI.Application.Imagenes.ImagenGet
{
    public record ImagenDTO(
        Guid? Id,
        string? Url,
        Guid? CursoId
    )
    {
        public ImagenDTO() : this(null, null, null){}
    }
}