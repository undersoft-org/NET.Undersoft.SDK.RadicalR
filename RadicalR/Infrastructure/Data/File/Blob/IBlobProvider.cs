using System.IO;
using System.Threading.Tasks;

namespace RadicalR
{
    public interface IBlobProvider
    {
        Task SaveAsync(BlobProviderSaveArgs args);

        Task<bool> DeleteAsync(BlobProviderArgs args);

        Task<bool> ExistsAsync(BlobProviderArgs args);

        Task<Stream> GetOrNullAsync(BlobProviderArgs args);
    }
}