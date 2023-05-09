using Microsoft.OData.Client;

namespace RadicalR
{
    public interface ILinkSynchronizer
    {
        void AddRepository(IRepository repository);

        void OnLinked(object sender, LoadCompletedEventArgs args);

        void AcquireLinker();

        void ReleaseLinker();

        void AcquireResult();

        void ReleaseResult();
    }
}

