using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class ProductDiscount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    // [BsonElement("Product")]
    public string ProductId { get; set; } = null!;

    public int DiscountValue { get; set; }

    public string DiscountUnit { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidUntil { get; set; }

    public string? CouponCode { get; set; }

    public decimal MinimumOrderValue { get; set; }

    public uint CouponLimit { get; set; }

    public bool IsRedeemable { get; set; }
}