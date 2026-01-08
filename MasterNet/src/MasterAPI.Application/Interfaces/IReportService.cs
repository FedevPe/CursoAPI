using MasterAPI.Domain.Models;

namespace MasterAPI.Application.Interfaces
{
    //Se define esta interfaz con el metodo GetCsvReport para luego poder exportar
    //datos desde la base de datos a un archivo .csv. Se utiliza un generico ya que,
    //esto permite que el metodo pueda recibir una lista de objetos de diferentes clases.
    //y el constraint (regla) indica que ese generico tiene como base la clase abstracta BaseEntity,
    //Por lo tanto, el metodo puede recibir una lista de objetos de la clase, cursos, instructores, precios,
    //calificaciones.
    public interface IReportService<T> where T : BaseEntity //Constraint
    {
        //MemoryStream: permite almacenar en memoria el stream (secuencia de bytes)
        //Dicho simple:
        //un stream es una forma secuencial de leer y/o escribir datos, sin importar de dónde vienen o a dónde van.
        //que tiene como ventaja ante un objeto normal que tiene un bajo consumo de memoria, siendo ideal para 
        //archivos con gran cantidad de datos.
        Task<MemoryStream> GetCsvReport(List<T> records);
    }
}