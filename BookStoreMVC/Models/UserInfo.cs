using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class UserInfo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Gender { get; set; } = null!;
    
    public IList<UserAddress> Address { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string Role { get; set; } = null!;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Country { get; set; }

    public string PhoneNumber { get; set; } = null!;
}