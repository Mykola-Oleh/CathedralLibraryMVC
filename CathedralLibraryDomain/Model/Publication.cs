using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Publication : Entity<int>
{
    public int PublicationId { get; set; }

    public string Title { get; set; } = null!;

    public string Anotaion { get; set; } = null!;

    public string PublicationType { get; set; } = null!;

    public short Year { get; set; }

    public virtual ICollection<Copy> Copies { get; set; } = new List<Copy>();

    public virtual ICollection<Fundchange> Fundchanges { get; set; } = new List<Fundchange>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
}
