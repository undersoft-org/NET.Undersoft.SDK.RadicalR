//-----------------------------------------------------------------------
// <copyright file="StoreCache.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Series;
using System.Uniques;

namespace RadicalR
{
    public class StoreCache<TStore> : DataCache, IStoreCache<TStore>
    {
        public StoreCache(IDataCache cache)
        {
            if((base.cache == null) || (this.cache == null))
            {
                Mapper = ServiceManager.GetObject<IDataMapper>();
                base.cache = cache;
                uint seed = typeof(TStore).UniqueKey32();
                UniqueType = seed;
                if(!base.Catalog.TryGet(seed, out IUnique deck))
                {
                    deck = new MassCatalog<IUnique>();
                    base.Catalog.Add(seed, deck);
                }
                this.cache = (IMassDeck<IUnique>)deck;
            }
        }

        public StoreCache(TimeSpan? lifeTime = null, Deputy callback = null, int capacity = 257) : base(
            lifeTime,
            callback,
            capacity)
        {
            if(cache == null)
            {
                uint seed = typeof(TStore).UniqueKey32();
                UniqueType = seed;
                if(!base.Catalog.TryGet(seed, out IUnique deck))
                {
                    deck = new MassCatalog<IUnique>();
                    base.Catalog.Add(seed, deck);
                }
                cache = (IMassDeck<IUnique>)deck;
            }
        }

        protected override IMassDeck<IUnique> cache { get; set; }

        public override IMassDeck<IUnique> Catalog => cache;

        public IDataMapper Mapper { get; set; }
    }
}
