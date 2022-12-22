using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string BookId { get; set; } = null!;

    // public decimal Cost { get; set; }

    public PriceStruct BasePrice { get; set; } = null!;

    public PriceStruct CurrentPrice { get; set; } = null!;

    public DateTime CreatedDate { get; set; }
    
    public DateTime ExpireDate { get; set; }

    // [BsonRepresentation(BsonType.ObjectId)]
    // public IList<string>? Reviews { get; set; }

    public float AverageScore { get; set; }

    public IEnumerable<string>? Reviews { get; set; }
}