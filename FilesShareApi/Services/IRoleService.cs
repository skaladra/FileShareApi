using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IRoleService
    {
        Task<IdentityResult> AddUserToRole(string id, string role);
        Task<IdentityResult> CreateRole(string role);
        IEnumerable<RoleEntity> GetRoles();
    }
}
