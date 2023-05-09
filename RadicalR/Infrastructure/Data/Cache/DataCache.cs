using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Instant;
using System.Linq;
using System.Series;
using System.Threading.Tasks;
using System.Uniques;

namespace RadicalR
{
    public class DataCache : MassCache<IUnique>, IDataCache
    {
        public DataCache() : this(TimeSpan.FromMinutes(15), null, 259)
        {
        }

        public DataCache(TimeSpan? lifeTime = null, IDeputy callback = null, int capacity = 259) : base(
            lifeTime,
            callback,
            capacity)
        {
            if (cache == null)
            {
                cache = this;
            }
        }

        protected virtual IMassDeck<IUnique> cache { get; set; }

        protected override T InnerMemorize<T>(T item)
        {
            uint group = typeof(T).ProxyRetypeKey32();
            if (!cache.TryGet(group, out IUnique deck))
            {
                Sleeve sleeve = SleeveFactory.Create(typeof(T).ProxyRetype(), group);
                sleeve.Combine();

                IRubrics keyrubrics = AssignKeyRubrics(sleeve, item);

                ISleeve isleeve = item.ToSleeve();

                deck = new MassCatalog<IUnique>();

                foreach (MemberRubric keyRubric in keyrubrics)
                {
                    Catalog<IUnique> subdeck = new Catalog<IUnique>();

                    subdeck.Add(item);

                    ((IMassDeck<IUnique>)deck).Put(
                        isleeve[keyRubric.RubricId],
                        keyRubric.RubricName.UniqueKey32(),
                        subdeck);
                }

                cache.Add(group, deck);

                cache.Add(item);

                return item;
            }

            if (!cache.ContainsKey(item))
            {
                IMassDeck<IUnique> _deck = (IMassDeck<IUnique>)deck;

                ISleeve isleeve = item.ToSleeve();

                foreach (MemberRubric keyRubric in isleeve.Rubrics.KeyRubrics)
                {
                    if (!_deck.TryGet(
                        isleeve[keyRubric.RubricId],
                        keyRubric.RubricName.UniqueKey32(),
                        out IUnique outdeck))
                    {
                        outdeck = new Catalog<IUnique>();

                        ((IDeck<IUnique>)outdeck).Put(item);

                        _deck.Put(isleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), outdeck);
                    }
                    else
                    {
                        ((IDeck<IUnique>)outdeck).Put(item);
                    }
                }
                cache.Add(item);
            }

            return item;
        }

        protected override T InnerMemorize<T>(T item, params string[] names) 
        {
            Memorize(item);

            ISleeve sleeve = item.ToSleeve();

            MemberRubric[] keyrubrics = sleeve.Rubrics.Where(p => names.Contains(p.RubricName)).ToArray();

            IMassDeck<IUnique> _deck = (IMassDeck<IUnique>)cache.Get(item.UniqueType);

            foreach (MemberRubric keyRubric in keyrubrics)
            {
                if (!_deck.TryGet(sleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), out IUnique outdeck))
                {
                    outdeck = new Catalog<IUnique>();

                    ((IDeck<IUnique>)outdeck).Put(item);

                    _deck.Put(sleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), outdeck);
                }
                else
                {
                    ((IDeck<IUnique>)outdeck).Put(item);
                }
            }

            return item;
        }

        public static IRubrics AssignKeyRubrics(Sleeve sleeve, IUnique item)
        {
            if (!sleeve.Rubrics.KeyRubrics.Any())
            {
                IEnumerable<bool[]>[] rk = item.GetIdentities()
                    .AsCards()
                    .Select(
                        p => (p.Key != (int)DbIdentityType.NONE)
                            ? p.Value
                                .Select(
                                    e => new[]
                                            {
                                                sleeve.Rubrics[e.Name].IsKey = true,
                                                sleeve.Rubrics[e.Name].IsIdentity = true
                                            })
                            : p.Value.Select(h => new[] { sleeve.Rubrics[h.Name].IsIdentity = true }))
                    .ToArray();

                sleeve.Rubrics.KeyRubrics.Put(sleeve.Rubrics.Where(p => p.IsIdentity == true).ToArray());
                sleeve.Rubrics.Update();
            }

            return sleeve.Rubrics.KeyRubrics;
        }

        public override T Memorize<T>(T item)
        {
            if (!item.IsEventType())
                return InnerMemorize(item);
            return default(T);
        }

        public virtual IMassDeck<IUnique> Catalog => cache;

        public override uint GetValidTypeKey(Type obj)
        {
            return obj.ProxyRetypeKey32();
        }
        public override Type GetValidType(Type obj)
        {
            return obj.ProxyRetype();
        }
        public override uint GetValidTypeKey(object obj)
        {
            return obj.ProxyRetypeKey32();
        }
        public override Type GetValidType(object obj)
        {
            return obj.ProxyRetype();
        }
    }
}
