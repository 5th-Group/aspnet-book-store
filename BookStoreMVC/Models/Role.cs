using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace BookStoreMVC.Models;

[CollectionName("ApplicationRoles")]
public class Role : MongoIdentityRole<Guid>
{
    // public string Name { get; set; }
}