using AutoMapper;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Extract;
using System.Instant;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Uniques;

namespace RadicalR
{
    [DataContract]
    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public class Identifiable : ValueProxy, IIdentifiable
    {
        private Uscn uniquecode;
        private int[] keyOrdinals;

        public Identifiable() : this(true) { }

        public Identifiable(bool autoId)
        {
            if (!autoId)
                return;

            uniquecode.UniqueKey = Unique.New;
            IsNewKey = true;
        }

        public Identifiable(object id) : this(id.UniqueKey64()) { }

        public Identifiable(ulong id) : this()
        {
            uniquecode.UniqueKey = id;
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        [IgnoreMap]
        [FigureAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public Uscn UniqueCode
        {
            get => uniquecode;
            set => uniquecode = value;
        }

        [FigureKey]
        [DataMember(Order = 1)]
        [Column(Order = 1)]
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual long Id
        {
            get => (long)UniqueKey;
            set => UniqueKey = (ulong)value;
        }

        [DataMember(Order = 2)]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Ordinal { get; set; }

        [Column(Order = 3)]
        [StringLength(32)]
        [DataMember(Order = 3)]
        [FigureAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public virtual string Label { get; set; }

        [Column(Order = 4)]
        [DataMember(Order = 4)]
        public virtual long SourceId { get; set; }

        [Column(Order = 5)]
        [StringLength(128)]
        [DataMember(Order = 5)]
        [FigureAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public virtual string SourceType { get; set; }

        [Column(Order = 6)]
        [DataMember(Order = 6)]
        public virtual long TargetId { get; set; }

        [Column(Order = 7)]
        [StringLength(128)]
        [DataMember(Order = 7)]
        [FigureAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public virtual string TargetType { get; set; }

        [Required]
        [FigureIdentity]
        [StringLength(32)]
        [ConcurrencyCheck]
        [DataMember(Order = 199)]
        [Column(Order = 199)]
        [FigureAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public virtual string CodeNumber
        {
            get => uniquecode;
            set => uniquecode.FromHexTetraChars(value.ToCharArray());
        }


        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        private bool IsNewKey { get; set; }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual byte Flags
        {
            get => (byte)uniquecode.GetFlags();
            set => uniquecode.SetFlagBits(new BitVector32(value));
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        [IgnoreMap]
        public virtual bool Inactive
        {
            get => GetFlag(1);
            set => SetFlag(value, 1);
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        [IgnoreMap]
        public virtual bool Locked
        {
            get => GetFlag(0);
            set => SetFlag(value, 0);
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        [IgnoreMap]
        public virtual int TypeKey
        {
            get => (int)UniqueType;
            set => UniqueType = (uint)value;
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        [IgnoreMap]
        public override int OriginKey
        {
            get { return (int)uniquecode.UniqueOrigin; }
            set { uniquecode.UniqueOrigin = (uint)value; }
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        [IgnoreMap]
        public virtual bool Obsolete
        {
            get => GetFlag(2);
            set => SetFlag(value, 2);
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        [IgnoreMap]
        public virtual byte Priority
        {
            get => GetPriority();
            set => SetPriority(value);
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        [IgnoreMap]
        public virtual DateTime Time
        {
            get => DateTime.FromBinary(uniquecode.Time);
            set => uniquecode.Time = value.ToBinary();
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        [IgnoreMap]
        public override ulong UniqueKey
        {
            get => uniquecode.UniqueKey;
            set
            {
                if ((value != 0) && !uniquecode.Equals(value) && IsNewKey)
                {
                    uniquecode.UniqueKey = value;
                    IsNewKey = false;
                }
            }
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        [IgnoreMap]
        public override ulong UniqueType
        {
            get =>
                (uniquecode.UniqueType == 0)
                    ? (uniquecode.UniqueType = this.ProxyRetypeKey32())
                    : uniquecode.UniqueType;
            set
            {
                if ((value != 0) && (value != uniquecode.UniqueType))
                    uniquecode.UniqueType = this.ProxyRetypeKey32();
            }
        }

        public long AutoId()
        {
            ulong key = uniquecode.UniqueKey;
            if (key != 0)
                return (long)key;

            ulong id = Unique.New;
            uniquecode.UniqueKey = id;
            return (long)id;
        }

        public void ClearFlag(ushort position)
        {
            uniquecode.ClearFlagBit(position);
        }

        public virtual ulong CompactKey()
        {
            return UniqueValues().UniqueKey64();
        }

        public byte ComparePriority(IIdentifiable entity)
        {
            return uniquecode.ComparePriority(entity.GetPriority());
        }

        public int CompareTo(IIdentifiable other)
        {
            return uniquecode.CompareTo(other);
        }

        public override int CompareTo(IUnique other)
        {
            return uniquecode.CompareTo(other);
        }

        public bool Equals(BitVector32 other)
        {
            return ((IEquatable<BitVector32>)uniquecode).Equals(other);
        }

        public bool Equals(DateTime other)
        {
            return ((IEquatable<DateTime>)uniquecode).Equals(other);
        }

        public bool Equals(IIdentifiable other)
        {
            return uniquecode.Equals(other);
        }

        public bool Equals(ISerialNumber other)
        {
            return ((IEquatable<ISerialNumber>)uniquecode).Equals(other);
        }

        public override bool Equals(IUnique other)
        {
            return uniquecode.Equals(other);
        }

        public override byte[] GetBytes()
        {
            return this.GetStructureBytes();
        }

        public bool GetFlag(ushort position)
        {
            return uniquecode.GetFlagBit(position);
        }

        public byte GetPriority()
        {
            return uniquecode.GetPriority();
        }

        public override byte[] GetUniqueBytes()
        {
            return uniquecode.GetUniqueBytes();
        }

        public void SetFlag(ushort position)
        {
            uniquecode.SetFlagBit(position);
        }

        public void SetFlag(bool flag, ushort position)
        {
            uniquecode.SetFlag(flag, position);
        }

        public long SetId(object id)
        {
            if (id == null)
                return AutoId();
            else if (id.GetType().IsPrimitive)
                return SetId((long)id);
            else
                return SetId((long)id.UniqueKey64());
        }

        public long SetId(long id)
        {
            ulong ulongid = (ulong)id;
            ulong key = uniquecode.UniqueKey;
            if ((ulongid != 0) && (key != ulongid))
                return (long)(UniqueKey = ulongid);
            return AutoId();
        }

        public byte SetPriority(byte priority)
        {
            return uniquecode.SetPriority(priority);
        }

        public IIdentifiable Sign()
        {
            return Sign(this);
        }

        public TEntity Sign<TEntity>() where TEntity : class, IIdentifiable
        {
            return Sign((TEntity)((object)this));
        }

        public TEntity Sign<TEntity>(TEntity entity) where TEntity : class, IIdentifiable
        {
            entity.AutoId();
            Stamp(entity);
            Created = Time;
            return entity;
        }

        public IIdentifiable Sign(object id)
        {
            return Sign(this, id);
        }

        public TEntity Sign<TEntity>(object id) where TEntity : class, IIdentifiable
        {
            return Sign((TEntity)((object)this));
        }

        public TEntity Sign<TEntity>(TEntity entity, object id) where TEntity : class, IIdentifiable
        {
            entity.SetId(id);
            Stamp(entity);
            Created = Time;
            return entity;
        }

        public IIdentifiable Stamp()
        {
            return Stamp(this);
        }

        public TEntity Stamp<TEntity>() where TEntity : class, IIdentifiable
        {
            return Stamp((TEntity)((object)this));
        }

        public TEntity Stamp<TEntity>(TEntity entity) where TEntity : class, IIdentifiable
        {
            //if (!entity.IsGUID)
            entity.Time = DateTime.Now;
            return entity;
        }

        public virtual int[] UniqueOrdinals()
        {
            if (keyOrdinals == null)
            {
                IRubrics pks = ((IValueProxy)this).Valuator.Rubrics.KeyRubrics;
                if (pks.Any())
                {
                    keyOrdinals = pks.Select(p => p.RubricId).ToArray();
                }
            }
            return keyOrdinals;
        }

        public virtual object[] UniqueValues()
        {
            int[] ids = keyOrdinals;
            if (ids == null)
                ids = UniqueOrdinals();
            return ids.Select(k => this[k]).ToArray();
        }


    }
}
