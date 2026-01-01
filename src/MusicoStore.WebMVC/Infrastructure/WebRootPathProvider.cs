using MusicoStore.Domain.Interfaces.Infrastructure;

namespace WebMVC.Infrastructure;

public sealed class WebRootPathProvider(IWebHostEnvironment env) : IStoragePathProvider
{
    public string RootPath => env.WebRootPath;
}
