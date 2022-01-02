using MongoDbGenericRepository.Attributes;
using System;
using AspNetCore.Identity.MongoDbCore.Models;

namespace FilesShareApi
{
    [CollectionName("Roles")]
    public class RoleEntity : MongoIdentityRole<Guid>
    {
    }
}
