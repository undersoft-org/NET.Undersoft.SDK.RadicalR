namespace RadicalR
{
    public interface IRemoteRepositoryLink<TStore, TOrigin, TTarget> : IRemoteRepository<TStore, TTarget>,
                     IRemoteLink<TOrigin, TTarget>, ILinkedObject<TStore, TOrigin>
                     where TOrigin : Entity where TTarget : Entity where TStore : IDataStore
    {
    }
}
