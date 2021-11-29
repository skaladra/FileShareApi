using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public class UserEntity
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
