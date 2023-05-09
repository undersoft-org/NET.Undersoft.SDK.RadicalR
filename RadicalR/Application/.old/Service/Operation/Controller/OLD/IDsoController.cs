using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using System.ServiceModel;
using System.Uniques;

namespace RadicalR
{
    [ServiceContract]
    public interface IDsoController<TKey, TStore, TEntity> where TEntity : Entity where TStore : IDataServiceStore
    {
        Task<IActionResult> Delete([FromODataUri] TKey key);
        IQueryable Get();
        UniqueOne Get([FromODataUri] TKey key);
        Task<IActionResult> Patch([FromODataUri] TKey key, TEntity entity);
        Task<IActionResult> Post(TEntity entity);
        Task<IActionResult> Put([FromODataUri] TKey key, TEntity entity);
    }
}