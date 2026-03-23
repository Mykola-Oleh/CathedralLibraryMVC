using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Wishlist : Entity<Guid>
{
    public Guid ReaderId { get; set; }
    public Guid PublicationId { get; set; }

    public virtual Publication Publication { get; set; } = null!;
    public virtual Reader Reader { get; set; } = null!;
}
