using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using System;
using AspNetCore.Identity.MongoDbCore.Models;
using System.ComponentModel.DataAnnotations;

namespace FilesShareApi
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
    }
}
