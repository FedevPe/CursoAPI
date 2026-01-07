namespace MasterAPI.Domain.Models
{
    // Esta clase representa las políticas de autorización disponibles en la aplicación. 
    // Es decir, define los permisos que los usuarios pueden tener para realizar diversas acciones.
    public static class PolicyMaster
    {
        public const string CURSO_CREATE = nameof(CURSO_CREATE);
        public const string CURSO_READ = nameof(CURSO_READ);
        public const string CURSO_UPDATE = nameof(CURSO_UPDATE);
        public const string CURSO_DELETE = nameof(CURSO_DELETE);

        public const string INSTRUCTOR_CREATE = nameof(INSTRUCTOR_CREATE);
        public const string INSTRUCTOR_READ = nameof(INSTRUCTOR_READ);
        public const string INSTRUCTOR_UPDATE = nameof(INSTRUCTOR_UPDATE);
        public const string INSTRUCTOR_DELETE = nameof(INSTRUCTOR_DELETE);

        public const string COMENTARIO_CREATE = nameof(COMENTARIO_CREATE);
        public const string COMENTARIO_READ = nameof(COMENTARIO_READ);
        public const string COMENTARIO_UPDATE = nameof(COMENTARIO_UPDATE);
        public const string COMENTARIO_DELETE = nameof(COMENTARIO_DELETE);
    }
}