using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class Review
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Comment { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? Reviewer { get; set; }

    public float RatedScore { get; set; }

}