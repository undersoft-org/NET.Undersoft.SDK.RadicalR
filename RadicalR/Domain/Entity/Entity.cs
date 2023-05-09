using System.ComponentModel;
using System.Instant;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Uniques;

namespace RadicalR
{
    [DataContract]
    [StructLayout(LayoutKind.Sequential)]
    public class Entity : Identifiable, IEntity, INotifyPropertyChanged
    {
        public Entity()
        {
            CompileValuator((v) =>
            {
                Type type = this.ProxyRetype();

                if (UniqueType == 0)
                    UniqueType = type.UniqueKey32();

                if (type.IsAssignableTo(typeof(ISleeve)) || type.IsAssignableTo(typeof(Event)))
                    return;

                v.Valuator = SleeveFactory.Create(type, (uint)UniqueType).Combine(this);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
