using MongoDB.Driver;
using MongoDB.Bson;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.Mongo.Logging;

public class MongoLoggingRepository : ILoggingRepository
{
    private readonly IMongoCollection<MongoLogEntry> _collection;

    public MongoLoggingRepository(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase(settings.DatabaseName);
        _collection = db.GetCollection<MongoLogEntry>(settings.LogCollectionName);
    }

    public async Task AddAsync(string method, string path, string body, CancellationToken ct)
    {
        var entry = new MongoLogEntry
        {
            Method = method,
            Path = path,
            Body = body,
            CreatedAt = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(entry, cancellationToken: ct);
    }
}

public static class MongoLoggingProbe
{
    public static bool CanConnect(MongoDbSettings settings, out Exception? error)
    {
        try
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            db.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            error = ex;
            return false;
        }
    }
}

public sealed class NoOpLoggingRepository : ILoggingRepository
{
    public Task AddAsync(string method, string path, string body, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
