using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Domain.Dto;

public class MongoFile
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public FileType? Type { get; set; }

    public DateTime Date { get; set; }

    public string Data { get; set; }
}