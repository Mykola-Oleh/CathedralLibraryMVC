using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CathedralLibraryDomain.Model;

public partial class Author : Entity<Guid>
{
    [Display(Name = "Ім'я автора")]
    public string FirstName { get; set; } = null!;
    [Display(Name = "Прізвище автора")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Автор")]
    public string ShortName => $"{(!string.IsNullOrEmpty(FirstName) ? FirstName[0] + "." : "")} {LastName}";
    public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
}
