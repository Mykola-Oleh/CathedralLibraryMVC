using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Wishlist : Entity<int>
{
    public int ReaderId { get; set; }

    public int? PublicationId { get; set; }

    public virtual Publication? Publication { get; set; }

    public virtual Reader Reader { get; set; } = null!;
}
