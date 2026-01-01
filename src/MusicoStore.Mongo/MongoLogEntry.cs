using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MusicoStore.Mongo.Logging;

public class MongoLogEntry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Method { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Body { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
