using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CathedralLibraryDomain.Model;

public partial class Publication : Entity<Guid>
{
    [Display (Name ="Назва видання")]
    [Required(ErrorMessage ="Поле 'Назва' не повинно бути порожнім")]
    [StringLength(255, ErrorMessage = "Поле 'Назва статусу' не може перевищувати 255 символів")]
    public string Title { get; set; } = null!;

    [Display(Name = "Анотація")]
    [Required(ErrorMessage = "Поле 'Анотація' не повинно бути порожнім")]
    [StringLength(255, ErrorMessage = "Поле 'Назва статусу' не може перевищувати 255 символів")]
    public string Annotation { get; set; } = null!;

    [Display(Name = "Тип видання")]
    [Required(ErrorMessage = "Поле 'Тип видання' не повинно бути порожнім")]
    public int PublicationTypeId { get; set; }

    [Display(Name = "Тип публікації")]
    public virtual PublicationType? PublicationType { get; set; } = null!;

    [Display(Name = "Рік")]
    public short Year { get; set; }

    public virtual ICollection<Copy> Copies { get; set; } = new List<Copy>();
    public virtual ICollection<Fundchange> Fundchanges { get; set; } = new List<Fundchange>();
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
}