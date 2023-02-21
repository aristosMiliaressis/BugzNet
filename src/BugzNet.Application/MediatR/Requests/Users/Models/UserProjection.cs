using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugzNet.Application.MediatR.Requests.Users.Models
{
    public class UserProjection
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}
