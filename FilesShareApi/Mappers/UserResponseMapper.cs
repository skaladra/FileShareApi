using System.Collections.Generic;

namespace FilesShareApi
{
    public static class UserResponseMapper
    {
        public static List<UserResponseDto> CreateDtoList(IEnumerable<UserEntity> users)
        {
            var result = new List<UserResponseDto>();
            foreach (var user in users)
            {
                result.Add(CreateUserResponseDto(user));
            }
            return result;
        }

        public static UserResponseDto CreateUserResponseDto(UserEntity user)
        {
            return new UserResponseDto()
            {
                CreatedOn = user.CreatedOn,
                Email = user.Email,
                Roles = user.Roles,
                Id = user.Id,
                UserName = user.UserName
            };
        }
    }
}
