namespace CathedralLibraryInfrastructure.ViewModels
{
    public class EditProfileViewModel
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Faculty { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Position { get; set; } = null!;
        public int Year { get; set; }
    }
}
