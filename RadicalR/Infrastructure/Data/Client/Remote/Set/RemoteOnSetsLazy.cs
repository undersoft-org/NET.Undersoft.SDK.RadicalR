using System.Collections;
using System.Collections.ObjectModel;
using System.Series;

namespace RadicalR
{
    public class RemoteOnSetsLazy<TEntity, TRemote> : Lazy<RemoteSet<TRemote>>, ICollection<TRemote>, IEnumerable<TRemote>, IFindable,
                                        IEnumerable, IList<TRemote>, IReadOnlyCollection<TRemote>, IReadOnlyList<TRemote>,
                                        ICollection, IList where TRemote : class, IIdentifiable where TEntity : class, IIdentifiable
    {
        private RemoteSet<TRemote> table => Value;

        public int Count => table.Count;

        public bool IsReadOnly => ((ICollection<TRemote>)table).IsReadOnly;

        public bool IsSynchronized => ((ICollection)table).IsSynchronized;

        public object SyncRoot => ((ICollection)table).SyncRoot;

        public bool IsFixedSize => ((IList)table).IsFixedSize;

        object IList.this[int index] { get => ((IList)table)[index]; set => ((IList)table)[index] = value; }
        public TRemote this[int index] { get => ((Collection<TRemote>)table)[index]; set => ((Collection<TRemote>)table)[index] = value; }

        public RemoteOnSetsLazy()
        {
        }

        public RemoteOnSetsLazy(bool isThreadSafe) : base(isThreadSafe)
        {
        }

        public RemoteOnSetsLazy(TEntity entity, Func<TEntity, Func<RemoteSet<TRemote>>> valueFactory) : base(valueFactory(entity))
        {
        }

        public RemoteOnSetsLazy(Func<RemoteSet<TRemote>> valueFactory) : base(valueFactory)
        {
        }

        public RemoteOnSetsLazy(Func<RemoteSet<TRemote>> valueFactory, bool isThreadSafe) : base(valueFactory, isThreadSafe)
        {
        }

        public RemoteOnSetsLazy(Func<RemoteSet<TRemote>> valueFactory, LazyThreadSafetyMode mode) : base(valueFactory, mode)
        {
        }

        public RemoteOnSetsLazy(LazyThreadSafetyMode mode) : base(mode)
        {
        }

        public RemoteOnSetsLazy(RemoteSet<TRemote> value) : base(value)
        {
        }

        public TRemote this[long key]
        {
            get
            {
                return (TRemote)this[(object)key];
            }
        }
        public object this[object key]
        {
            get
            {
                return table[key];
            }
            set
            {
                table[key] = value;
            }
        }

        public int IndexOf(TRemote item)
        {
            return table.IndexOf(item);
        }

        public void Insert(int index, TRemote item)
        {
            table.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            table.RemoveAt(index);
        }

        public void Add(TRemote item)
        {
            table.Add(item);
        }

        public void Clear()
        {
            table.Clear();
        }

        public bool Contains(TRemote item)
        {
            return table.Contains(item);
        }

        public void CopyTo(TRemote[] array, int arrayIndex)
        {
            table.CopyTo(array, arrayIndex);
        }

        public bool Remove(TRemote item)
        {
            return table.Remove(item);
        }

        public IEnumerator<TRemote> GetEnumerator()
        {
            return table.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)table).GetEnumerator();
        }

        void ICollection<TRemote>.CopyTo(TRemote[] array, int index)
        {
            table.CopyTo(array, index);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)table).CopyTo(array, index);
        }

        public int Add(object value)
        {
            return ((IList)table).Add(value);
        }

        public bool Contains(object value)
        {
            return ((IList)table).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IList)table).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            ((IList)table).Insert(index, value);
        }

        public void Remove(object value)
        {
            ((IList)table).Remove(value);
        }
    }
}