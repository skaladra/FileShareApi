using System.Collections.Generic;


namespace FilesShareApi
{
    public static class UserChatMapper
    {
        public static List<UserChatEntityDto> CreateDtoList(List<UserEntity> users)
        {
            var dtoList = new List<UserChatEntityDto>();
            foreach (var user in users)
            {
                dtoList.Add(CreateUserCharDto(user));
            }
            return dtoList;
        }

        public static UserChatEntityDto CreateUserCharDto(UserEntity user)
        {
            return new UserChatEntityDto()
            {
                Id = user.Id.ToString(),
                Name = user.UserName
            };
        }
    }
}
