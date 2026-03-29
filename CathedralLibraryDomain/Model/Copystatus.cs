using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CathedralLibraryDomain.Model;

[Table("copystatus")]
public partial class Copystatus : Entity<int>
{
    [Display(Name = "Назва статусу")]
    [StringLength(32, ErrorMessage = "Поле 'Назва статусу' не може перевищувати 32 символів")]
    public string StatusName { get; set; } = null!;

    public virtual ICollection<Copy> Copies { get; set; } = new List<Copy>();
}
