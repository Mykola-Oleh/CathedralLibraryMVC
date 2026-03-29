using System;
using System.Collections.Generic;
using System.Text;

namespace CathedralLibraryDomain.Model
{
    public class PublicationType:Entity<int>
    {
        public string Name { get; set; }

        // Зворотний зв'язок: один тип може бути у багатьох публікацій
        public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
    }
}
