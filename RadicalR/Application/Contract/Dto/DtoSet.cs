using System.Collections.ObjectModel;
using System.Linq;
using System.Series;
using System.Uniques;

namespace RadicalR
{

    public class DtoSet<TDto> : KeyedCollection<long, TDto>, IFindable where TDto : IUniqueObject
    {
        protected override long GetKeyForItem(TDto item)
        {
            return (item.Id == 0) ? (long)item.AutoId() : item.Id;
        }

        public TDto Single
        {
            get => this.FirstOrDefault();
        }

        public object this[object key]
        {
            get
            {
                TryGetValue((long)(key.UniqueKey64()), out TDto result);
                return result;
            }
            set
            {
                Dictionary[(long)(key.UniqueKey64())] = (TDto)value;
            }
        }
    }
}