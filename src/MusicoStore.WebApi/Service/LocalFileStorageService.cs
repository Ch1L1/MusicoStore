using MusicoStore.Domain.DTOs;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.WebApi.Service;

// This service is located inside WebApi, because we're using the wwwroot folder to store product images.
public class LocalFileStorageService(IWebHostEnvironment env) : IFileStorageService
{
    private string WebRoot { get; } = env.WebRootPath;

    public async Task<string> SaveFileAsync(FileDTO file, string folderName, CancellationToken ct)
    {
        var uploadsFolder = Path.Combine(WebRoot, folderName);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{extension}";
        var physicalPath = Path.Combine(uploadsFolder, fileName);

        await using (var fileStream = new FileStream(physicalPath, FileMode.Create))
        {
            await file.Content.CopyToAsync(fileStream, ct);
        }

        return Path.Combine(folderName, fileName).Replace("\\", "/");
    }

    public void DeleteFile(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            return;
        }

        var physicalPath = Path.Combine(WebRoot, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

        if (!File.Exists(physicalPath))
        {
            return;
        }
        
        File.Delete(physicalPath);
    }
}
