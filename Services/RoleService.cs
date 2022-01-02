using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi.Services
{
    public class RoleService: IRoleService
    {
        private readonly RoleManager<RoleEntity> roleManager;
        private readonly UserManager<UserEntity> userManager;

        public RoleService(RoleManager<RoleEntity> roleManager, UserManager<UserEntity> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task<IdentityResult> AddUserToRole(string id, string role)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return IdentityResult.Failed();
            var result = await userManager.AddToRoleAsync(user, role);
            return result;
        }

        public async Task<IdentityResult> CreateRole(string role)
        {
            var result = await roleManager.CreateAsync(new RoleEntity() { Name = role });
            return result;
        }

        public IEnumerable<RoleEntity> GetRoles()
        {
            return roleManager.Roles;
        }
    }
}
