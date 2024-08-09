using System;
using System.Collections.Generic;

namespace AuthenticationWebApi.Common.Data
{
    public partial class User
    {
        public User()
        {
            Roles = new HashSet<Role>();
        }

        public Guid Id { get; set; }
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public float? Balance { get; set; }
        public bool Active { get; set; }
        public Guid? CurrenRoomId { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
