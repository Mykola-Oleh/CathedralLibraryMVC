using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Request : Entity<Guid>
{
    public Guid ReaderId { get; set; }
    public Guid PublicationId { get; set; }
    public int Copynumber { get; set; }
    public Guid StatusId { get; set; }

    public DateTime RequestDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public virtual Copy Copy { get; set; } = null!;
    public virtual Reader Reader { get; set; } = null!;
    public virtual Requeststatus Status { get; set; } = null!;
}
