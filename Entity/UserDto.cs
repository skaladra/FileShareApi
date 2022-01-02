using System.ComponentModel.DataAnnotations;

namespace FilesShareApi
{
    public class UserDto
    {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Email for login
        /// </summary>
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
