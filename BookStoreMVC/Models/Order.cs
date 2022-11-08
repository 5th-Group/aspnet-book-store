using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("productList")]
    public IList<OrderProducts> ProductList { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string Status { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string Customer { get; set; } = null!;

    public decimal TotalPrice { get; set; }
}