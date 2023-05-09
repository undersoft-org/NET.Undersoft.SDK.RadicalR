using System.Diagnostics.CodeAnalysis;
using System.Threading;
using JetBrains.Annotations;


namespace RadicalR
{
    public class BlobProviderArgs
    {
        [DisallowNull]
        public string ContainerName { get; }

        [DisallowNull]
        public BlobContainerConfiguration Configuration { get; }

        [DisallowNull]
        public string BlobName { get; }

        public CancellationToken CancellationToken { get; }

        public BlobProviderArgs(
            [DisallowNull] string containerName,
            [DisallowNull] BlobContainerConfiguration configuration,
            [DisallowNull] string blobName,
            CancellationToken cancellationToken = default)
        {
            ContainerName = containerName;
            Configuration = configuration;
            BlobName = blobName;
            CancellationToken = cancellationToken;
        }
    }
}