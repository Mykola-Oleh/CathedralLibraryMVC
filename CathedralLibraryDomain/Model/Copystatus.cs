using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Copystatus : Entity<Guid>
{
    public string StatusName { get; set; } = null!;

    public virtual ICollection<Copy> Copies { get; set; } = new List<Copy>();
}
