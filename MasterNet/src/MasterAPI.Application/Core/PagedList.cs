using Microsoft.EntityFrameworkCore;

namespace MasterAPI.Application.Core
{
    public class PagedList<T>
    {
        public PagedList(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(totalCount/(double)pageSize);
            PageSize = pageSize;
            TotalCount = totalCount;
            Items = items;
        }

        public int CurrentPage { get; set;}
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; } //Respresenta el total de items
        public List<T> Items { get; set;} = new List<T>();

        //Este metodo es el que se encarga de crear la paginacion, y es generica para que se pueda utilizar con cualquier entidad.
        public static async Task<PagedList<T>> CreateAsync(
            IQueryable<T> source, //Representa la consulta en terminos de expression funtions
            int pageNumber,
            int pageSize
        )
        {
            var count = await source.CountAsync(); //Esta linea de codigo ya realiza la consulta contra la DB, obteniendo la cantidad de registros
            var items = await source
                        .Skip((pageNumber-1)*pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}