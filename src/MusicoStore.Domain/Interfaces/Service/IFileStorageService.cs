using MusicoStore.Domain.DTOs;

namespace MusicoStore.Domain.Interfaces.Service;

public interface IFileStorageService
{
    /// <summary>
    /// Saves a file and returns a relative path (e.g. "images/products/guid.jpg")
    /// </summary>
    Task<string> SaveFileAsync(FileDTO file, string folderName, CancellationToken ct);

    /// <summary>
    /// Deletes a file based on relative path
    /// </summary>
    void DeleteFile(string relativePath);
}
