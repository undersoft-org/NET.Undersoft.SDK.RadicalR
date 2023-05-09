using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.ObjectModel;
using System.Instant;
using System.Linq;
using System.Series;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class Identifiers<TEntity> : KeyedCollection<long, Identifier<TEntity>>, IFindable<Identifier<TEntity>>, IIdentifiers where TEntity : Identifiable
    {
        [JsonIgnore] private static IDeck<Identifier<TEntity>> _identifiers = new Catalog<Identifier<TEntity>>(true);

        IIdentifiable IIdentifiers.this[object id, IdKind kind]
        {
            get => _identifiers.GetCard(id).FirstOrDefault(o => o.Kind == kind);
            set => ((Identifier<TEntity>)value).PutTo(_identifiers.GetCard(id).FirstOrDefault(o => o.Kind == kind));
        }
        IIdentifiable IIdentifiers.this[object id]
        {
            get => this[id];
            set => ((IFindable)this)[id] = value;
        }

        object IFindable.this[object key]
        {
            get => _identifiers[key];
            set => ((IFindable)_identifiers)[key] = value;
        }

        public Identifiers() : base()
        {
        }

        public Identifier<TEntity> Search(object id)
        {
            return _identifiers[id];
        }

        public Identifier<TEntity> this[object id]
        {
            get => _identifiers[id];
            set => _identifiers[id] = value;
        }

        protected override long GetKeyForItem(Identifier<TEntity> item)
        {
            if (item.Id == 0)
            {
                item.AutoId();
            }

            return item.Id;
        }

        protected override void InsertItem(int index, Identifier<TEntity> item)
        {
            _identifiers.Put(item.Id, item);
            _identifiers.Put((ulong)item.Key, item);

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            _identifiers.Remove((ulong)this[index].Key);

            base.RemoveItem(index);
        }

        IIdentifiable IIdentifiers.Search(object id)
        {
            return Search(id);
        }
    }

    public interface IIdentifiers
    {
        IIdentifiable Search(object id);

        IIdentifiable this[object id] { get; set; }

        IIdentifiable this[object id, IdKind kind] { get; set; }
    }

    public class IdentifiersMapping<TEntity> where TEntity : Entity
    {
        private string TABLE_NAME = typeof(TEntity).Name + "Identifiers";
        private readonly ModelBuilder _modelBuilder;
        private readonly EntityTypeBuilder<TEntity> _entityBuilder;
        private readonly EntityTypeBuilder<Identifier<TEntity>> _identifierBuilder;

        public IdentifiersMapping(ModelBuilder builder)
        {
            _modelBuilder = builder;
            _entityBuilder = _modelBuilder.Entity<TEntity>();
            _identifierBuilder = _modelBuilder.Entity<Identifier<TEntity>>();
        }

        public ModelBuilder Configure()
        {
            _identifierBuilder.ToTable(TABLE_NAME, DataBaseSchema.IdentifierSchema);

            _identifierBuilder.HasIndex(k => k.Key);

            _identifierBuilder.HasOne(a => a.Entity)
                              .WithMany("Identifiers")
                              .HasForeignKey(k => k.EntityId)
                              .IsRequired()
                              .OnDelete(DeleteBehavior.Restrict);

            _entityBuilder.HasMany("Identifiers")
                          .WithOne(nameof(Identifier<TEntity>.Entity))
                          .HasForeignKey(nameof(Identifier<TEntity>.EntityId))
                          .IsRequired()
                          .OnDelete(DeleteBehavior.Restrict);

            _entityBuilder.Navigation("Identifiers");

            return _modelBuilder;
        }
    }
}