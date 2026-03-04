using System.Net;
using System.Text.Json;
using MasterAPI.Application.Core;
using Newtonsoft.Json;

namespace MasterAPI.WebAPI.Middleware
{
    public class ExceptionMiddleware
    {
        //Este continua con el ciclo de vida del request, es un hijo de ejecución
        private readonly RequestDelegate _next;
        //
        private readonly ILogger<ExceptionMiddleware> _logger;
        //Este campo captura el ambiente donde se esta ejecutando la aplicación, de desarrollo, producción, etc
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware (
            RequestDelegate next, 
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        //Este es el elemento mas importante de un middleware, evalua si dentro del ciclo de vida del request
        //se dispara una excepción.
        public async Task InvokeAsync (HttpContext context)
        {
            //Se envuelve todo el ciclo de vida del request en un try-catch
            try
            {
                //Esta linea de codigo lo que hace es seguir el hijo de ejecución durante el ciclo de vida del request
                await _next(context);
            }
            //Si dentro del ciclo de vida del request, surge algun error en algun componente, es capturado por el bloque catch
            catch (Exception ex)
            {

                //Configuracion para que trabaje con ValidationException (MENSAJES DE VALIDACION PERSONALIZADA)

                _logger.LogError(ex, ex.Message);

                var response = ex switch
                {
                    ValidationException validation => new AppException(
                        StatusCodes.Status400BadRequest,
                        "Error de validación",
                        string.Join(", ", validation.Errors!.Select(er => er.ErrorMessage))
                        // JsonConvert.SerializeObject(validation.Errors!.ToArray())
                    ),
                    _ => new AppException(
                        context.Response.StatusCode,
                        ex.Message,
                        ex.StackTrace!.ToString()
                    )
                };

                context.Response.StatusCode = response.StatusCode;
                context.Response.ContentType = "application/json";
                var json = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(json);


                // //Lo primero que se hace es imprimir el error con el logger.
                // _logger.LogError (ex, ex.Message);
                // //Desde aqui se construye lo que se devuelce al cliente. Para que sea facilmente entendible.
                // //Esta linea de comando indica el tipo de formato que se va a devolver, en este caso JSON
                // context.Response.ContentType = "application/json";
                // //Se retorna un codigo de error al cliente.
                // context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // //Aqui se puede enviar el error dependiendo del ambiente del proyecto, evaluando si se encuentra en
                // //produccion o desarrollo.
                // var response = _env.IsDevelopment() ? 
                //     //Si esta en ambiente de desarrollo
                //     new AppException(
                //         context.Response.StatusCode, 
                //         ex.Message,
                //         //Esta linea de codigo, envia todo el mensaje de error como un string.
                //         ex.StackTrace?.ToString())
                //     : //Si no esta en desarrollo
                //     new AppException(
                //         context.Response.StatusCode, 
                //         "Internal Server Error");

                // //Y por ultimo se debe serializar el mensaje, osea convertirlo a formato JSON.
                // var options = new JsonSerializerOptions
                // {
                //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                // };
                
                // var json = JsonSerializer.Serialize (response, options);

                // await context.Response.WriteAsync(json);

                

            }
        }
    }
}