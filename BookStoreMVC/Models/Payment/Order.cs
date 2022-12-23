using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models.Payment;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public IList<ProductListItem> ProductList { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public string? PaymentStatus { get; set; }

    public IEnumerable<OrderStatus> ShippingStatus { get; set; } = null!;
    public int CurrentShippingStatus { get; set; }

    public string ShippingAddress { get; set; } = null!;

    [BsonRepresentation(BsonType.String)]
    public string Customer { get; set; } = null!;

    public decimal TotalPrice { get; set; }
}