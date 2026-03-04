
namespace MasterAPI.Application.Core
{
    public class Result<T>
    {
        public bool IsSucces { get; set; }
        //Esta propiedad representa el valor que se va a retornar cuando IsSucces es true
        public T? Value { get; set; }
        //Cuando ocurre un error, esta propiedad va a mostrarlo
        public string? Error { get; set; }

        //En esta seccion sirve para crear objetos de la clase Result, esta misma, cuando la aplicacion comienza a ejecutarse.
        //Por eso es que se una static.
        //Cuando la operacion es exitosa se crea un objeto y se asigna el valor true a isSucces
        public static Result<T> Succes (T value) => new Result<T>
        {
            IsSucces = true, 
            Value = value
        };
        //Cuando ocurre un error en la operacion se crea un objeto asigando el valor false a isSucces y
        //Un valor a la propiedad error, que sirve para mostrar un error personalizado.
        public static Result<T> Failure (string error) => new Result<T>
        {
            IsSucces = false,
            Error = error
        };
    }
}