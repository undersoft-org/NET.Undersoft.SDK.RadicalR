using System.Instant;
using System.Uniques;

namespace RadicalR
{
    public enum Towards
    {
        None,
        ToSet,
        ToSingle,
        SetToSet
    }

    public interface IRemoteLink : IUnique
    {
        Towards Towards { get; set; }

        MemberRubric LinkedMember { get; }
    }

    public abstract class RemoteLink : IRemoteLink
    {
        protected Uscn serialcode;

        protected RemoteLink() { }

        public virtual Towards Towards { get; set; }

        public virtual IUnique Empty => Uscn.Empty;

        public virtual Uscn SerialCode
        {
            get => serialcode;
            set => serialcode = value;
        }

        public virtual ulong UniqueKey
        {
            get => serialcode.UniqueKey;
            set => serialcode.UniqueKey = value;
        }
        public virtual ulong UniqueType
        {
            get => serialcode.UniqueType;
            set => serialcode.UniqueType = value;
        }

        public virtual int CompareTo(IUnique other)
        {
            return serialcode.CompareTo(other);
        }

        public virtual bool Equals(IUnique other)
        {
            return serialcode.Equals(other);
        }

        public virtual byte[] GetBytes()
        {
            return serialcode.GetBytes();
        }

        public virtual byte[] GetUniqueBytes()
        {
            return serialcode.GetUniqueBytes();
        }

        public virtual MemberRubric LinkedMember { get; }
    }
}
