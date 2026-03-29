using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CathedralLibraryDomain.Model
{
    public class User : IdentityUser
    {
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public int Year { get; set; }
    }
}
