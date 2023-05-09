using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

using System.Logs;

namespace RadicalR
{
    public class DataBaseContext<TStore> : DataBaseContext, IDataBaseContext<TStore> where TStore : IDataStore
    {
        protected virtual Type StoreType { get; }

        public DataBaseContext(DbContextOptions options, IRadicalr radicalr = null) : base(options, radicalr)
        {
            StoreType = typeof(TStore);
        }
    }

    public class DataBaseContext : DbContext, IDataBaseContext, IResettableService
    {
        public virtual IRadicalr radicalr { get; }

        public override IModel Model
        {
            get
            {
                return base.Model;
            }
        }

        public DataBaseContext(DbContextOptions options, IRadicalr radicalr = null) : base(options)
        {
            this.radicalr = radicalr;
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class, IIdentifiable
        {
            return this.Set<TEntity>();
        }

        public object EntitySet<TEntity>() where TEntity : class, IIdentifiable
        {
            return this.Set<TEntity>();
        }

        public object EntitySet(Type type)
        {
            return this.GetDbSet(type);
        }

        public virtual Task<int> Save(bool asTransaction, CancellationToken token = default(CancellationToken))
        {
            return saveEndpoint(asTransaction, token);
        }

        private async Task<int> saveEndpoint(bool asTransaction, CancellationToken token = default(CancellationToken))
        {
            if (this.ChangeTracker.HasChanges())
            {
                if (asTransaction)
                    return await saveAsTransaction(token);
                else
                    return await saveChanges(token);
            }
            return 0;
        }

        private async Task<int> saveAsTransaction(CancellationToken token = default(CancellationToken))
        {
            await using var tr = await this.Database.BeginTransactionAsync(token);
            try
            {
                var changes = await this.SaveChangesAsync(token);

                await tr.CommitAsync(token);

                return changes;
            }
            catch (DbUpdateException e)
            {
                if (e is DbUpdateConcurrencyException)
                    tr.Warning<Datalog>($"Concurrency update exception data changed by: {e.Source}, " +
                                        $"entries involved in detail data object", e.Entries, e);
                else
                    tr.Failure<Datalog>(
                        $"Fail on update database transaction Id:{tr.TransactionId}, using context:{this.GetType().Name}," +
                        $" context Id:{this.ContextId}, TimeStamp:{DateTime.Now.ToString()} {e.StackTrace}", e.Entries);

                await tr.RollbackAsync(token);

                tr.Warning<Datalog>($"Transaction Id:{tr.TransactionId} Rolling Back !!");
            }
            return -1;
        }

        private async Task<int> saveChanges(CancellationToken token = default(CancellationToken))
        {
            try
            {
                return await this.SaveChangesAsync(token);
            }
            catch (DbUpdateException e)
            {
                if (e is DbUpdateConcurrencyException)
                    this.Warning<Datalog>($"Concurrency update exception data changed by: {e.Source}, " +
                                             $"entries involved in detail data object", e.Entries, e);
                else
                    this.Failure<Datalog>(
                        $"Fail on update database, using context:{this.GetType().Name}, " +
                        $"context Id: {this.ContextId}, TimeStamp: {DateTime.Now.ToString()}");
            }

            return -1;
        }
    }
}