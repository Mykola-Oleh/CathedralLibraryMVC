using CathedralLibraryDomain.Model;

namespace CathedralLibraryInfrastructure.ViewModels
{
    public class AdminToolViewModel
    {
        public IEnumerable<PublicationType> PublicationTypes { get; set; }
        public IEnumerable<Requeststatus> RequestStatuses { get; set; }
        public IEnumerable<Copystatus> CopyStatuses { get; set; }
        public string ActiveTab { get; set; }
    }
}
