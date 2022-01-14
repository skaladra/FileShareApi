using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
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
