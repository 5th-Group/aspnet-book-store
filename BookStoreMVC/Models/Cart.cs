using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class Cart
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public IEnumerable<ProductList> Items { get; set; } = null!;

    // public TYPE Type { get; set; }

    public DateTime CreatedAt { get; set; }
    
}