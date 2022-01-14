namespace FilesShareApi
{
    public class UserLoginMapper
    {
        public static UserLoginDto CreateUserLoginDto(string email, string password)
        {
            return new UserLoginDto()
            {
                Email = email,
                Password = password
            };
        }
    }
}
