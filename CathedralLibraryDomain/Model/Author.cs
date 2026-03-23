using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Author : Entity<Guid>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
}
