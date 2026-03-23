using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Copy
{
    public Guid PublicationId { get; set; }
    public int Copynumber { get; set; }
    public Guid StatusId { get; set; }

    public virtual Publication Publication { get; set; } = null!;
    public virtual Copystatus Status { get; set; } = null!;
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
