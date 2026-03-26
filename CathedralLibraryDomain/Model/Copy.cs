using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CathedralLibraryDomain.Model;

public partial class Copy
{
    [Display(Name = "Назва видання")]
    public Guid PublicationId { get; set; }

    [Display(Name = "Номер копії")]
    [Range(1, 9999, ErrorMessage = "Поле 'Номер копії' має бути додатним числом (більше 0) і меншим за 10000")]
    public int Copynumber { get; set; }
    [Display(Name = "Статус")]
    public Guid StatusId { get; set; }

    public virtual Publication Publication { get; set; } = null!;
    public virtual Copystatus Status { get; set; } = null!;
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
