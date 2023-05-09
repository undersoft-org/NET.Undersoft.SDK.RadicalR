using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RadicalR
{
    public class EnetityLinkSetToRemoteSet<TLeft, TRight>
        where TLeft : Entity
        where TRight : Entity
    {
        private readonly string RELATION_TABLE_NAME;
        private readonly string LEFT_TABLE_NAME = typeof(TLeft).Name + "s";
        private readonly string RIGHT_TABLE_NAME = typeof(TRight).Name + "s";
        private readonly string LEFT_NAME = typeof(TLeft).Name + "s";
        private readonly string RIGHT_NAME =
            typeof(TRight).Name.Replace(typeof(TLeft).Name, "") + "s";
        private readonly string LEFT_SCHEMA = null;
        private readonly string RIGHT_SCHEMA = null;

        private readonly ExpandSite _expandSite;
        private readonly ModelBuilder _modelBuilder;
        private readonly EntityTypeBuilder<TLeft> _firstBuilder;
        private readonly EntityTypeBuilder<TRight> _secondBuilder;
        private readonly EntityTypeBuilder<EntityLink<TLeft, TRight>> _relationBuilder;

        public EnetityLinkSetToRemoteSet(
            ModelBuilder modelBuilder,
            ExpandSite expandSite = ExpandSite.None,
            string dbSchema = null
        ) : this(modelBuilder, null, null, null, null, expandSite, dbSchema, dbSchema) { }

        public EnetityLinkSetToRemoteSet(
            ModelBuilder modelBuilder,
            string leftName,
            string rightName,
            ExpandSite expandSite = ExpandSite.None,
            string dbSchema = null
        )
            : this(
                modelBuilder,
                leftName,
                leftName,
                rightName,
                rightName,
                expandSite,
                dbSchema,
                dbSchema
            )
        { }

        public EnetityLinkSetToRemoteSet(
            ModelBuilder modelBuilder,
            string leftName,
            string leftTableName,
            string rightName,
            string rightTableName,
            ExpandSite expandSite = ExpandSite.None,
            string parentSchema = null,
            string childSchema = null
        )
        {
            _modelBuilder = modelBuilder;
            _firstBuilder = _modelBuilder.Entity<TLeft>();
            _secondBuilder = _modelBuilder.Entity<TRight>();
            _relationBuilder = _modelBuilder.Entity<EntityLink<TLeft, TRight>>();
            _expandSite = expandSite;

            if (leftTableName != null)
                LEFT_TABLE_NAME = leftTableName;
            if (rightTableName != null)
                RIGHT_TABLE_NAME = rightTableName;
            if (leftName != null)
                LEFT_NAME = leftName;
            if (rightName != null)
                RIGHT_NAME = rightName;
            if (parentSchema != null)
                LEFT_SCHEMA = parentSchema;
            if (childSchema != null)
                RIGHT_SCHEMA = childSchema;

            RELATION_TABLE_NAME = LEFT_NAME + "To" + RIGHT_NAME;
        }

        public ModelBuilder Configure()
        {
            _relationBuilder.ToTable(RELATION_TABLE_NAME, DataBaseSchema.RelationSchema);
            _relationBuilder.HasKey(k => new { k.LeftEntityId, k.RightEntityId });

            _relationBuilder
                .HasOne(a => a.RightEntity)
                .WithMany(RELATION_TABLE_NAME)
                .HasForeignKey(a => a.RightEntityId);

            _relationBuilder
                .HasOne(a => a.LeftEntity)
                .WithMany(RELATION_TABLE_NAME)
                .HasForeignKey(a => a.LeftEntityId);

            _firstBuilder
                .HasMany<EntityLink<TLeft, TRight>>(RELATION_TABLE_NAME)
                .WithOne(p => p.LeftEntity)
                .HasForeignKey(k => k.LeftEntityId);

            _secondBuilder
                .HasMany<EntityLink<TLeft, TRight>>(RELATION_TABLE_NAME)
                .WithOne(p => p.RightEntity)
                .HasForeignKey(k => k.RightEntityId);

            if ((_expandSite & (ExpandSite.OnRight | ExpandSite.WithMany)) > 0)
                _firstBuilder.Navigation(RELATION_TABLE_NAME);
            else
                _secondBuilder.Navigation(RELATION_TABLE_NAME);

            return _modelBuilder;
        }
    }
}
