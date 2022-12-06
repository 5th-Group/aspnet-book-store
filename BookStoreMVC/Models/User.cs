using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreMVC.Models;

public class User : MongoIdentityUser<Guid>
{
    public string Username { get; set; } = null!;
    
    public string Password { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Gender { get; set; } = null!;
    
    public IEnumerable<UserAddress> Address { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string Roles { get; set; } = null!;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Country { get; set; }

    public string PhoneNumber { get; set; } = null!;
}