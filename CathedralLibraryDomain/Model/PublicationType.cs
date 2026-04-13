namespace CathedralLibraryDomain.Model
{
    public class PublicationType : Entity<int>
    {
        public string Name { get; set; }

        public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
    }
}
