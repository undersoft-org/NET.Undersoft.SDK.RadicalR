using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Instant;
using System.Linq;
using System.Linq.Expressions;
using System.Logs;
using System.Series;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{

    public class RemoteRepository<TEntity> : Repository<TEntity>, IRemoteRepository<TEntity>
        where TEntity : class, IIdentifiable
    {
        IDeck<DataServiceRequest> _batchset;
        protected DataServiceQuery<TEntity> dsQuery;

        protected RemoteSet<TEntity> dsSet;


        public RemoteRepository()
        {
        }

        public RemoteRepository(IRepositoryClient repositorySource) : base(repositorySource)
        {
            dsSet = new RemoteSet<TEntity>(dsContext);
            dsQuery = dsSet.Query;

            Expression = Expression.Constant(this);
            Provider = new LinkedRepositoryQueryProvider<TEntity>(Query);
        }

        public RemoteRepository(DataClientContext context) : base(context)
        {
            if (dsContext != null)
            {
                dsSet = new RemoteSet<TEntity>(dsContext);
                dsQuery = dsSet.Query;
                Expression = Expression.Constant(this.AsEnumerable());
                Provider = new LinkedRepositoryQueryProvider<TEntity>(dsQuery);
            }
        }

        public RemoteRepository(IRepositoryContextPool context) : base(context)
        {
        }

        public RemoteRepository(IQueryProvider provider, Expression expression)
        {
            ElementType = typeof(TEntity);
            Provider = provider;
            Expression = expression;
        }

        public override TEntity this[params object[] keys]
        {
            get => find(keys);
            set
            {
                TEntity entity = find(keys);
                if (entity != null)
                {
                    value.PatchTo(entity, PatchingEvent);
                    //dsContext.UpdateObject(Stamp(entity), keys);
                    if (dsSet.ContainsKey(keys))
                        dsSet[keys] = Stamp(entity);
                    else
                        dsSet.Add(Stamp(entity));
                }
                else
                    dsSet.Add(Sign(entity));
                //dsContext.AddObject(Name, Sign(value));
            }
        }

        public override TEntity this[object[] keys, params Expression<Func<TEntity, object>>[] expanders]
        {
            get
            {
                DataServiceQuery<TEntity> query = dsContext.CreateQuery<TEntity>(KeyString(keys), true);
                if (expanders != null)
                    foreach (Expression<Func<TEntity, object>> expander in expanders)
                       query = query.Expand(expander);

                var entity = query.FirstOrDefault();
                if (entity != null)
                    if (dsSet.ContainsKey(keys))
                        dsSet[keys] = entity;
                    else
                        dsSet.Add(entity);
                return entity;
            }
            set
            {
                DataServiceQuery<TEntity> query = dsContext.CreateQuery<TEntity>(KeyString(keys), true);
                if (expanders != null)
                    if (expanders != null)
                        foreach (Expression<Func<TEntity, object>> expander in expanders)
                            query = query.Expand(expander);

                TEntity entity = query.FirstOrDefault();
                if (entity != null)
                {
                    value.PatchTo(Stamp(entity), PatchingEvent);
                    if (dsSet.ContainsKey(keys))
                        dsSet[keys] = Stamp(entity);
                    else
                        dsSet.Add(Stamp(entity));                   
                }
            }
        }

        public override object this[Expression<Func<TEntity, object>> selector, object[] keys, params Expression<Func<TEntity, object>>[] expanders]
        {
            get
            {
                DataServiceQuery<TEntity> query = dsContext.CreateQuery<TEntity>(KeyString(keys), true);
                if (expanders != null)
                    foreach (Expression<Func<TEntity, object>> expander in expanders)
                        query = query.Expand(expander);

                object entity = query.Select(selector).FirstOrDefault();
                if (entity != null)
                    if (dsSet.ContainsKey(keys))
                        dsSet[keys] = entity;
                    else
                        dsSet.Add((TEntity)entity);

                return entity;
            }
            set
            {
                DataServiceQuery<TEntity> query = dsContext.CreateQuery<TEntity>(KeyString(keys), true);
                if (expanders != null)
                    foreach (Expression<Func<TEntity, object>> expander in expanders)
                        query = query.Expand(expander);

                TEntity entity = query.Select(selector).Cast<TEntity>().FirstOrDefault();
                if (entity != null)
                {
                    value.PatchTo(Stamp(entity), PatchingEvent);
                    if (dsSet.ContainsKey(keys))
                        dsSet[keys] = entity;
                    else
                        dsSet.Add(entity);
                }
            }
        }

        private TEntity lookup(params object[] keys)
        {
            var item = cache.Lookup<TEntity>(keys);
            if (item != null)
                dsContext.AttachTo(Name, item);
            return item;
        }

        TEntity find(params object[] keys)
        {
            if (keys != null)
            {
                var item = lookup(keys);
                if (item == null)
                {
                    item = dsContext.CreateFunctionQuerySingle<TEntity>(string.Empty, KeyString(keys), true).GetValue();
                    dsSet.Add(Stamp(item));
                    //dsContext.AttachTo(Name, item);
                }
                return item;
            }
            return null;
        }

        Task<TEntity> findAsync(params object[] keys)
        {
            return Task.Run(async () =>
            {
                if (keys != null)
                {
                    var item = lookup(keys);
                    if (item == null)
                    {
                        item = await dsContext.CreateFunctionQuerySingle<TEntity>(string.Empty, KeyString(keys), true).GetValueAsync();
                        dsSet.Add(Stamp(item));
                    }
                    return item;
                }
                return null;
            });
        }

        DataServiceRequest findOne(params object[] keys)
        {
            if (keys != null)
            {
                if (_batchset == null)
                    _batchset = new Catalog<DataServiceRequest>();

                return _batchset.Put(keys, dsContext.CreateQuery<TEntity>(KeyString(keys), true)).Value;
            }
            return null;
        }

        protected override TEntity InnerSet(TEntity entity)
        {
            Stamp(entity);
            //dsContext.UpdateObject(Stamp(entity));           
            var _entity = dsSet[entity.Id];
            entity.PatchTo(_entity); 
            return entity;
        }

        protected override async Task<int> saveAsTransaction(CancellationToken token = default(CancellationToken))
        {
            try
            {
                return (await dsContext.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset, token)).Count();
            }
            catch (Exception e)
            {
                dsContext.Failure<Datalog>(
                    $"{$"Fail on update dataservice as singlechangeset, using context:{dsContext.GetType().Name}, "}{$"TimeStamp: {DateTime.Now.ToString()}"}");
            }

            return -1;
        }

        protected override async Task<int> saveChanges(CancellationToken token = default(CancellationToken))
        {
            try
            {
                return (await dsContext.SaveChangesAsync(
                    SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.ContinueOnError,
                    token)).Count();
            }
            catch (Exception e)
            {
                dsContext.Failure<Datalog>(
                    $"{$"Fail on update dataservice as independent operations, using context:{dsContext.GetType().Name}, "}{$"TimeStamp: {DateTime.Now.ToString()}"}");
            }

            return -1;
        }

        protected DataClientContext dsContext => (DataClientContext)InnerContext;

        public override object TracePatching(object item, string propertyName = null, Type type = null)
        {
            if (type == null)
            {
                dsContext.AttachTo(item.GetType().Name, item);                
                return item;
            }
            else
            {
                var qor = dsContext.LoadProperty(item, propertyName);
                var source = (ISleeve)item;
                var target = source[propertyName];
                if (target == null)
                    source[propertyName] = target = type.New();
                return target;
            }
        }

        public override TEntity Add(TEntity entity)
        {
            TEntity _entity = Sign(entity);
            dsSet.Add(_entity);
            return entity;
        }

        public override IEnumerable<TEntity> Add(IEnumerable<TEntity> entity)
        {
            foreach (TEntity e in entity)
                yield return Add(e);
        }

        public override IAsyncEnumerable<TEntity> AddAsync(IEnumerable<TEntity> entity) { return entity.ForEachAsync((e) => Add(e)); }

        public override TEntity Delete(TEntity entity)
        {
            if (dsSet.ContainsKey(entity.Id))
                dsSet.Remove(entity);
            else
                dsContext.DeleteObject(entity);
            return entity;
        }

        public override Task<TEntity> Find(params object[] keys) { return findAsync(keys); }

        public async Task<IEnumerable<TEntity>> FindMany(params object[] keys)
        {
            foreach (object key in keys)
            {
                if (key.GetType().IsAssignableTo(typeof(object[])))
                    findOne((object[])key);
                else
                    findOne(key);
            }
            return (await Context.ExecuteBatchAsync(_batchset.ToArray()))
                .SelectMany(o => o as QueryOperationResponse<TEntity>);
        }

        public string KeyString(params object[] keys)
        { return $"{Name}({((keys.Length > 1) ? keys.Aggregate(string.Empty, (a, b) => $"{a},{b}") : keys[0])})"; }

        public override TEntity NewEntry(params object[] parameters)
        {
            TEntity entity = Sign(typeof(TEntity).New<TEntity>(parameters));
            dsSet.Add(entity);
            //dsContext.AddObject(Name, entity);
            return entity;
        }

        public DataServiceQuerySingle<TEntity> QuerySingle(params object[] keys)
        {
            if (keys != null)
                return dsContext.CreateFunctionQuerySingle<TEntity>(string.Empty, KeyString(keys), true);
            return null;
        }

        public override IQueryable<TEntity> AsQueryable()
        {
            return Query;
        }

        public DataClientContext Context => dsContext;

        public override DataServiceQuery<TEntity> Query => dsContext.CreateQuery<TEntity>(Name, true);


    }
}
