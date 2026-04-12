using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CathedralLibraryDomain.Model;

[Table("requeststatus")]
public partial class Requeststatus : Entity<int>
{
    public string StatusName { get; set; } = null!;
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}