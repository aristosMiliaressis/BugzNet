using System.Collections.Generic;

namespace BugzNet.Infrastructure.DataEF.Seeding
{
    public class SeedConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public bool OnlyUsers { get; set; } = true;
        public bool DropAndRecreate { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        //public List<Bug> Bugs { get; set; } = new List<Bug>();

        public List<User> Users { get; set; } = new List<User>();
        

        public class User
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
        }
        
    }
}
