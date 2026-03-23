using System;
using System.Collections.Generic;
using System.Text;

namespace CathedralLibraryDomain.Model
{
    public abstract class Entity<T>
    {
        public T Id { get; set; } = default!;
    }
}
