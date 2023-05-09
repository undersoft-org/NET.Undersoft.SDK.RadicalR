using Microsoft.OData.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RadicalR
{
    public interface IRemoteRepository<TStore, TEntity> : IRemoteRepository<TEntity> where TEntity : class, IIdentifiable
    {        
    }
} 