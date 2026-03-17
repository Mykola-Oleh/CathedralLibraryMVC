using System;
using System.Collections.Generic;
using System.Text;

namespace CathedralLibraryDomain.Model
{
    public abstract class Entity<TId>
    {
        public virtual TId Id { get; set; } = default!;
    }
}
