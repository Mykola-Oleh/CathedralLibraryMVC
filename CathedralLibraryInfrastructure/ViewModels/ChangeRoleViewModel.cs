using Microsoft.AspNetCore.Identity;

namespace CathedralLibraryInfrastructure.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }

        // Список усіх доступних ролей у системі (Admin, User, Librarian тощо)
        public List<IdentityRole> AllRoles { get; set; }

        // Список ролей, які вже призначені цьому користувачу
        public IList<string> UserRoles { get; set; }

        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
