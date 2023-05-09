using Microsoft.OData.Client;
using System.Instant;
using System.Linq.Expressions;

namespace RadicalR
{
    public class RemoteRepositoryLink<TStore, TOrigin, TTarget> : RemoteRepository<TStore, TTarget>, IRemoteRepositoryLink<TStore, TOrigin, TTarget>
        where TOrigin : Entity
        where TTarget : Entity
        where TStore : IDataStore
    {
        IRemoteLink<TOrigin, TTarget> relation;

        public RemoteRepositoryLink(
            IRepositoryContextPool<OpenClientContext<TStore>> pool,
            IEntityCache<TStore, TTarget> cache,
            IRemoteLink<TOrigin, TTarget> relation,
            ILinkSynchronizer synchronizer) : base(pool, cache)
        {
            this.relation = relation;
            Synchronizer = synchronizer;
        }

        public Expression<Func<TTarget, bool>> CreatePredicate(object entity)
        { return relation.CreatePredicate(entity); }

        public void Load(object origin) { Load(origin, dsContext); }

        public void Load<T>(IEnumerable<T> origins, DataClientContext context) where T : class
        { origins.DoEach((o) => Load(o, context)); }

        public void Load(object origin, DataClientContext context)
        {
            IEntity _entity = (IEntity)origin;
            int rubricId = LinkedMember.RubricId;

            Expression<Func<TTarget, bool>> predicate = CreatePredicate(origin);
            if (predicate != null)
            {
                IRemote<TTarget> dso;
                switch (Towards)
                {
                    case Towards.ToSingle:
                        DataServiceQuery<TTarget> query = context.CreateQuery<TTarget>(typeof(TTarget).Name);
                        Synchronizer.AcquireLinker();
                        _entity[rubricId] = query.FirstOrDefault(predicate);
                        Synchronizer.ReleaseLinker();
                        break;
                    case Towards.ToSet:
                        dso = typeof(RemoteOnSet<TTarget>).New<RemoteSet<TTarget>>(context);
                        dso.LoadCompleted += Synchronizer.OnLinked;
                        _entity[rubricId] = dso;
                        dso.LoadAsync(predicate);
                        Synchronizer.AcquireLinker();
                        break;
                    case Towards.SetToSet:
                        dso = typeof(RemoteOnSets<TTarget>).New<RemoteOnSets<TTarget>>(context);
                        dso.LoadCompleted += Synchronizer.OnLinked;
                        _entity[rubricId] = dso;
                        dso.LoadAsync(predicate);
                        Synchronizer.AcquireLinker();
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task LoadAsync(object origin) { await Task.Run(() => Load(origin, dsContext), Cancellation); }

        public async ValueTask LoadAsync(object origin, DataClientContext context, CancellationToken token)
        { await Task.Run(() => Load(origin, context), token); }

        public override Task<int> Save(bool asTransaction, CancellationToken token = default)
        { return ContextLease.Save(asTransaction, token); }

        public IRepository Host { get; set; }

        public bool IsLinked { get; set; }

        public override int LinkedCount { get; set; }

        public MemberRubric LinkedMember => relation.LinkedMember;

        public Expression<Func<TOrigin, object>> OriginKey
        {
            get => relation.OriginKey;
            set => relation.OriginKey = value;
        }

        public Func<TOrigin, Expression<Func<TTarget, bool>>> Predicate
        {
            get => relation.Predicate;
            set => relation.Predicate = value;
        }

        public ILinkSynchronizer Synchronizer { get; }

        public Expression<Func<TTarget, object>> TargetKey
        {
            get => relation.TargetKey;
            set => relation.TargetKey = value;
        }

        public override Towards Towards => relation.Towards;
    }
}
