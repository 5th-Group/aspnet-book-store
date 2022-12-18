using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class PriceStruct
{
    [BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
    public decimal Hardcover { get; set; }

    [BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
    public decimal Paperback { get; set; }

    [BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
    public decimal Ebook { get; set; }
}