using Microsoft.OData.Edm;
using System;

namespace RadicalR
{
    public partial class StreamClientContext<TStore> where TStore : IDataStore
    {
        public StreamClientContext(Uri serviceUri)
        {
        }
    }

    public partial class GrpcClientContext : IDataClient
    {
        public GrpcClientContext(Uri serviceUri)
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