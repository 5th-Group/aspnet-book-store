using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("Detail")]
    public string BookId { get; set; } = null!;

    public decimal Cost { get; set; }

    public decimal Price { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public IList<string>? Reviews { get; set; }
}