using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class ProductListItem
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductDetail { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }
    public decimal TotalPrice
    {
        get
        {
            return this.Price * this.Quantity;
        }
    }


}