using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CathedralLibraryDomain.Model;

public partial class Publication : Entity<Guid>
{
    public string Title { get; set; } = null!;
    public string Annotation { get; set; } = null!;
    public string PublicationType { get; set; } = null!;
    public short Year { get; set; }

    public virtual ICollection<Copy> Copies { get; set; } = new List<Copy>();
    public virtual ICollection<Fundchange> Fundchanges { get; set; } = new List<Fundchange>();
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
}