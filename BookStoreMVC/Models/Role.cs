using AspNetCore.Identity.MongoDbCore.Models;

namespace BookStoreMVC.Models;

public class Role : MongoIdentityRole<Guid>
{
    public string Name { get; set; }
}