using MusicoStore.Domain.Interfaces.Infrastructure;

namespace MusicoStore.WebApi.Infrastructure;

public sealed class WebRootPathProvider(IWebHostEnvironment env) : IStoragePathProvider
{
    public string RootPath => env.WebRootPath;
}
