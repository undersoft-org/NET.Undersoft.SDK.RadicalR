namespace RadicalR
{
    public interface IEventStore : IDataStore, IDataServiceStore
    {

    }

    public interface ICqrsStore : IDataServiceStore
    {

    }

    public interface IReportStore : ICqrsStore, IDataStore
    {

    }

    public interface IEntryStore : ICqrsStore, IDataStore
    {

    }

    public interface IDataStore
    {
    }

    public interface IDataServiceStore
    {
    }
}
