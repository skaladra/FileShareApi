using MongoDbGenericRepository.Attributes;
using System;
using AspNetCore.Identity.MongoDbCore.Models;

namespace FilesShareApi
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
    }
}
