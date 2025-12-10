using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.DTO
{
    public class RegistrationDto
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? HashedPassword { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public UserType? Type { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? ReaderBio { get; set; }
    }
}
