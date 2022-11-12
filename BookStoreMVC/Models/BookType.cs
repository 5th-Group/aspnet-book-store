using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;


// Model of Book Type Collection
public class BookType
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("type")]
    public string Type { get; set; } = null!;
    
}