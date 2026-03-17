using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Reader : Entity<int>
{
    public int ReaderId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Faculty { get; set; } = null!;

    public string Department { get; set; } = null!;

    public string Position { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual Wishlist? Wishlist { get; set; }
}
