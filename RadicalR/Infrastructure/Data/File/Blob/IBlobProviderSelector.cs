using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace RadicalR
{
    public interface IBlobProviderSelector
    {
        IBlobProvider Get([DisallowNull] string containerName);
    }
}