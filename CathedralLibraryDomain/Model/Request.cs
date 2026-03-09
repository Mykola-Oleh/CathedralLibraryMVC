using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Request
{
    public int RequestId { get; set; }

    public int ReaderId { get; set; }

    public int PublicationId { get; set; }

    public int Copynumber { get; set; }

    public int StatusId { get; set; }

    public DateOnly RequestDate { get; set; }

    public DateOnly ReturnDate { get; set; }

    public DateOnly DueDate { get; set; }

    public virtual Copy Copy { get; set; } = null!;

    public virtual Reader Reader { get; set; } = null!;

    public virtual Requeststatus Status { get; set; } = null!;
}
