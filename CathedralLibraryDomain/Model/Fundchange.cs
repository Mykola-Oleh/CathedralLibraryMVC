using System;
using System.Collections.Generic;

namespace CathedralLibraryDomain.Model;

public partial class Fundchange
{
    public int ChangeId { get; set; }

    public int PublicationId { get; set; }

    public string ChangeType { get; set; } = null!;

    public int Quantity { get; set; }

    public DateOnly ChangeDate { get; set; }

    public int ChangedByAdminId { get; set; }

    public virtual Publication Publication { get; set; } = null!;
}
