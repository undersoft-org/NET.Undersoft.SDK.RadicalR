namespace RadicalR
{
    public interface IIdentifier<TEntity> : IIdentifier
    {
        new long EntityId { get; set; }
        TEntity Entity { get; set; }
    }
    public interface IIdentifier
    {
        long Id { get; set; }

        long EntityId { get; set; }

        IdKind Kind { get; set; }

        string Name { get; set; }

        string Value { get; set; }

        long Key { get; }
    }
}