using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Copystatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Copy> Copies { get; set; } = new List<Copy>();
}
