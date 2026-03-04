using MasterAPI.Application.Core;

namespace MasterNet.Application.Precios.GetPrecios
{
    public class GetPreciosRequest : PagingParams
    {
        public string? Nombre {get;set;}
    }
    
}
