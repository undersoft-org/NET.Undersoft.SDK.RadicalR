using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Series;
using System.Threading.Tasks;
using System.Uniques;

namespace RadicalR
{
    public class EntityCache<TStore, TEntity> : StoreCache<TStore>, IEntityCache<TStore, TEntity>
        where TEntity : IUnique
    {
        public EntityCache(IStoreCache<TStore> datacache) : base()
        {
            if ((base.cache == null) || (cache == null))
            {
                Mapper = datacache.Mapper;
                base.cache = datacache;
                uint seed = typeof(TEntity).ProxyRetypeKey32();
                if (!base.Catalog.TryGet(seed, out IUnique deck))
                {
                    deck = new MassCatalog<IUnique>();
                    base.Catalog.Add(seed, deck);
                }
                cache = (IMassDeck<IUnique>)deck;

                UniqueType = seed;
                base.UniqueType = typeof(TStore).ProxyRetypeKey32();
            }
        }

        protected override IMassDeck<IUnique> cache { get; set; }

        public IMassDeck<IUnique> CacheSet()
        {
            return CacheSet<TEntity>();
        }

        public TEntity Lookup(object keys)
        {
            return Lookup<TEntity>(keys);
        }

        public IDeck<IUnique> Lookup(Tuple<string, object> valueNamePair)
        {
            return Lookup<TEntity>(
                (m) => (IDeck<IUnique>)m.Get(valueNamePair.Item2, valueNamePair.Item1.UniqueKey32())
            );
        }

        public IDeck<IUnique> Lookup(Func<IMassDeck<IUnique>, IDeck<IUnique>> selector)
        {
            return Lookup<TEntity>(selector);
        }

        public TEntity Lookup(TEntity item)
        {
            return Lookup<TEntity>(item);
        }

        public TEntity[] Lookup(object key, params Tuple<string, object>[] valueNamePairs)
        {
            return Lookup<TEntity>(key, valueNamePairs);
        }

        public TEntity[] Lookup(
            Func<IDeck<IUnique>, IUnique> key,
            params Func<IMassDeck<IUnique>, IDeck<IUnique>>[] selectors
        )
        {
            return Lookup<TEntity>(key, selectors);
        }

        public IDeck<IUnique> Lookup(object key, string propertyNames)
        {
            return Lookup<TEntity>(key, propertyNames);
        }

        public TEntity Lookup(TEntity item, params string[] propertyNames)
        {
            return Lookup<TEntity>(item, propertyNames);
        }

        public IEnumerable<TEntity> Memorize(IEnumerable<TEntity> items)
        {
            return Memorize<TEntity>(items);
        }

        public TEntity Memorize(TEntity item)
        {
            return Memorize<TEntity>(item);
        }

        public TEntity Memorize(TEntity item, params string[] names)
        {
            return Memorize<TEntity>(item, names);
        }

        public async Task<TEntity> MemorizeAsync(TEntity item)
        {
            return await MemorizeAsync<TEntity>(item);
        }

        public async Task<TEntity> MemorizeAsync(TEntity item, params string[] names)
        {
            return await MemorizeAsync<TEntity>(item, names);
        }

        public override IMassDeck<IUnique> Catalog => cache;

        public override ulong UniqueType { get; set; }
    }
}
