namespace MasterAPI.Domain.Models
{
    // Esta clase representa los claims personalizados disponibles en la aplicación. 
    // Es decir, define los diferentes tipos de políticas o acciones que pueden ser realizadas a los usuarios
    public static class CustomClaims
    {
        public const string POLICIES = nameof(POLICIES);
    }
}