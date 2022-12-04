using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class ProductList
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductDetail { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }
}