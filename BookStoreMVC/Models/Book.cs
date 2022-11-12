using BookStoreMVC.ViewModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

// Model of Book Collection
public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Title { get; set; } = null!;

    public int PageCount { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? Author { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? Language { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public IList<string>? Genre { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public IList<string>? Type { get; set; }

    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; }

    public string ImageUri { get; set; } = null!;

    [BsonRepresentation(BsonType.DateTime)]
    public DateTime PublishDate { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? Publisher { get; set; }

    public string? Isbn { get; set; }

    public string? Description { get; set; }
}