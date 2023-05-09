using Microsoft.OData.Edm;
using System;

namespace RadicalR
{
    public partial class CrudClientContext<TStore> where TStore : IDataStore
    {
        public CrudClientContext(Uri serviceUri)
        {
        }
    }

    public partial class WebApiClientContext : IDataClient
    {
        public WebApiClientContext(Uri serviceUri)
        {
        }

        public void CreateServiceModel()
        {

        }

        protected virtual IEdmModel OnModelCreating(IEdmModel builder)
        {
            return builder;
        }

    }
}