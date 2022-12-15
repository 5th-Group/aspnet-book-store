using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace BookStoreMVC.Models;

[CollectionName("ApplicationUsers")]
public class User : MongoIdentityUser<ObjectId>
{
    // public string Username { get; set; } = null!;
    //
    // public string Password { get; set; } = null!;
    // [BsonId]
    // [BsonRepresentation(BsonType.ObjectId)]
    // public override string? Id { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Gender { get; set; } = null!;
    
    public IEnumerable<UserAddress> Address { get; set; } = null!;
    
    // public string Email { get; set; } = null!;
    //
    // public string Roles { get; set; } = null!;
    
    // [BsonRepresentation(BsonType.ObjectId)]
    public string? Country { get; set; }

    // public string PhoneNumber { get; set; } = null!;
}