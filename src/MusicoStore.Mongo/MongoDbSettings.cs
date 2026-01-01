namespace MusicoStore.Mongo.Logging;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
    public string LogCollectionName { get; set; } = "Logs";
}
