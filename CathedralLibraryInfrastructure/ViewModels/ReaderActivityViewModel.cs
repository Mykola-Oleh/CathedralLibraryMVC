namespace CathedralLibraryInfrastructure.ViewModels
{
    public class ReaderActivityViewModel
    {
        public Guid ReaderId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int TotalBooksTaken { get; set; }
        public DateTime? LastRequestDate { get; set; }
        public string ActivityLevel { get; set; }
        public string ActivityColor { get; set; }
    }
}
