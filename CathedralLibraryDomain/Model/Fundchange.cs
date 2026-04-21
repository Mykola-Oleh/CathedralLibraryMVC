using System.ComponentModel.DataAnnotations.Schema;

namespace CathedralLibraryDomain.Model;

[Table("fundchanges")]
public partial class Fundchange : Entity<Guid>
{
    public Guid? PublicationId { get; set; }
    public string? PublicationTitle { get; set; }
    public string ChangeType { get; set; } = null!;

    public int Quantity { get; set; }

    public DateOnly ChangeDate { get; set; }
    public Guid? ChangedByAdminId { get; set; }

    [ForeignKey("PublicationId")]
    public virtual Publication? Publication { get; set; }
}
