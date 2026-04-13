using Microsoft.AspNetCore.Identity;

namespace CathedralLibraryDomain.Model
{
    public class User : IdentityUser
    {
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public int Year { get; set; }

        public virtual Reader? Reader { get; set; }
    }
}
