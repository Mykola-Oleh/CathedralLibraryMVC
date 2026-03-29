using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Requeststatus : Entity<int>
{
    public string StatusName { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
