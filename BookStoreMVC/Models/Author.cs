using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

// Model of Author Collection
public class Author
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string MiddleName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Origin { get; set; } = null!;
    
    public string Gender { get; set; } = null!;
    
    public DateTime DateOfBirth { get; set; }
    
    public string Biography { get; set; } = null!;
}