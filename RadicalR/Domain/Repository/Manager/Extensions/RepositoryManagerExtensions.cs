using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Series;
using System.Threading.Tasks;

namespace RadicalR
{
    public static class RepositoryManagerExtensions
    {             
        public static async Task LoadClientEdms(this AppSetup app)
        {
            await Task.Run(() =>
            {
                RepositoryManager.Clients.ForEach((client) =>
                {
                    client.BuildMetadata();
                });

                AppSetup.AddRuntimeImplementations();
                app.RebuildProviders();
            });
        }     

    }
}
