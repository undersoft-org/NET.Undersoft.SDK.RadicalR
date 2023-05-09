using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class Servicer : ServiceManager, IServicer, IMediator
    {
        new bool disposedValue;
        protected IMediator mediator;

        public Servicer() : base() { }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    base.Dispose(true);
                }
                disposedValue = true;
            }
        }

        protected IMediator Mediator => mediator ??= GetService<IMediator>();

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default
        )
        {
            return mediator.CreateStream(request, cancellationToken);
        }

        public IAsyncEnumerable<object> CreateStream(
            object request,
            CancellationToken cancellationToken = default
        )
        {
            return mediator.CreateStream(request, cancellationToken);
        }

        public Lazy<R> Deserve<T, R>(Func<T, R> function)
            where T : class
            where R : class
        {
            return new Lazy<R>(function.Invoke(GetService<T>()));
        }

        public override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore().ConfigureAwait(false);
        }

        public async Task Publish(
            object notification,
            CancellationToken cancellationToken = default
        )
        {
            await Serve<IMediator>(async (m) => await m.Publish(notification, cancellationToken));
        }

        public async Task Publish<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default
        ) where TNotification : INotification
        {
            await Serve<IMediator>(async (m) => await m.Publish(notification, cancellationToken));
        }

        public Task<R> Run<T, R>(Func<T, Task<R>> function) where T : class
        {
            return Task.Run(async () => await function.Invoke(GetService<T>()));
        }

        public Task Run<T>(Func<T, Task> function) where T : class
        {
            return Task.Run(async () => await function.Invoke(GetService<T>()));
        }

        public Task Run<T>(string methodname, params object[] parameters)
        {
            Deputy deputy = new Deputy(GetOrNewService<T>(), methodname);
            return deputy.ExecuteAsync(parameters);
        }

        public Task<R> Run<T, R>(string methodname, params object[] parameters) where T : class
        {
            Deputy deputy = new Deputy(GetOrNewService<T>(), methodname);
            return deputy.ExecuteAsync<R>(parameters);
        }

        public async Task Save(bool asTransaction = false)
        {
            await SaveEndpoints(true);
            await SaveClients(true);
        }

        public async Task<int> SaveClient(IRepositoryClient client, bool asTransaction = false)
        {
            return await client.Save(asTransaction);
        }

        public async Task<int> SaveClients(bool asTransaction = false)
        {
            int changes = 0;
            for (int i = 0; i < Clients.Count; i++)
            {
                changes += await SaveClient(Clients[i], asTransaction);
            }

            return changes;
        }

        public async Task<int> SaveEndpoint(
            IRepositoryEndpoint endpoint,
            bool asTransaction = false
        )
        {
            return await endpoint.Save(asTransaction);
        }

        public async Task<int> SaveEndpoints(bool asTransaction = false)
        {
            int changes = 0;
            for (int i = 0; i < Endpoints.Count; i++)
            {
                changes += await SaveEndpoint(Endpoints[i], asTransaction);
            }

            return changes;
        }

        public async Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default
        )
        {
            return await Mediator.Send(request, cancellationToken);
        }

        public async Task<object> Send(
            object request,
            CancellationToken cancellationToken = default
        )
        {
            return await Mediator.Send(request, cancellationToken);
        }

        public Task<R> Serve<T, R>(Func<T, Task<R>> function) where T : class
        {
            return Task.Run(async () =>
            {
                using (Radicalr us = new Radicalr())
                {
                    return await function.Invoke(us.GetService<T>());
                }
            });
        }

        public Task Serve<T>(Func<T, Task> function) where T : class
        {
            return Task.Run(async () =>
            {
                using (Radicalr us = new Radicalr())
                {
                    await function.Invoke(us.GetService<T>());
                }
            });
        }

        public Task Serve<T>(string methodname, params object[] parameters)
        {
            return Task.Run(() =>
            {
                using (Servicer us = new Servicer())
                {
                    Deputy deputy = new Deputy(us.GetOrNewService<T>(), methodname);
                    return deputy.Execute(parameters);
                }
            });
        }

        public Task<R> Serve<T, R>(string methodname, params object[] parameters) where T : class
        {
            return Task.Run(async () =>
            {
                using (Servicer us = new Servicer())
                {
                    Deputy deputy = new Deputy(us.GetOrNewService<T>(), methodname);
                    return await deputy.ExecuteAsync<R>(parameters);
                }
            });
        }
    }
}
