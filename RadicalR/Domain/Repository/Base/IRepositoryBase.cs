using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace RadicalR
{
    public interface IRepository : IRepositoryContext
    {       
        Type ElementType { get;  }

        Expression Expression { get;  }        

        IQueryProvider Provider { get; }

        IDataMapper Mapper { get; }

        CancellationToken Cancellation { get; set; }

        IEnumerable<ILinkedObject> LinkedObjects { get; set; }

        void LoadLinked(object entity);

        Task LoadLinkedAsync(object entity);

        void LoadRelated(EntityEntry entry, RelatedType relatedType);

        void LoadRelatedAsync(EntityEntry entry, RelatedType relatedType, CancellationToken token);
    }
}