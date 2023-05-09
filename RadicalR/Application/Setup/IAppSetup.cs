namespace RadicalR
{
    public interface IAppSetup
    {
        IAppSetup UseDataClients();

        IAppSetup UseInternalProvider();

        IAppSetup UseDataMigrations();
    }
}