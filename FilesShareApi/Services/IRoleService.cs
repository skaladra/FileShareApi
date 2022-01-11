using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IRoleService
    {
        Task<IdentityResult> AddUser(string id, string role);
        Task<IdentityResult> CreateOne(string role);
        IEnumerable<RoleEntity> GetAll();
    }
}
