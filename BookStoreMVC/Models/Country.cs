using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class Country
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } 
    
    [BsonElement("name")]
    public string Name { get; set; } = null!;
    
    [BsonElement("code")]
    public string Code { get; set; } = null!;
}