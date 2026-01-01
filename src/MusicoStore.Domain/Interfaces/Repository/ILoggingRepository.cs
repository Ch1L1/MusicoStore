namespace MusicoStore.Domain.Interfaces.Repository;

public interface ILoggingRepository
{
    Task AddAsync(string method, string path, string body, CancellationToken ct);
}
