using System;
using System.Collections.Generic;

namespace FilesShareApi
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTime CreatedOn { get; set; }

        public List<Guid> Roles { get; set; }
    }
}
