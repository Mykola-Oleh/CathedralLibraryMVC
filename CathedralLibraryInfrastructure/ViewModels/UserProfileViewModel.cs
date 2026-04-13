using CathedralLibraryDomain.Model;

namespace CathedralLibraryInfrastructure.ViewModels
{
    public class UserProfileViewModel
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Faculty { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Position { get; set; } = null!;
        public int Year { get; set; }

        public List<Request> ActiveRequests { get; set; } = new();
    }
}
