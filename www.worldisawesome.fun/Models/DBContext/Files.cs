using System;
using System.Collections.Generic;

namespace www.worldisawesome.fun.DataModels
{
    public partial class Files
    {
        public Files()
        {
            Experiences = new HashSet<Experiences>();
            Users = new HashSet<Users>();
            Places = new HashSet<Places>();
        }

        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public DateTimeOffset FileCreated { get; set; }

        public virtual ICollection<Experiences> Experiences { get; set; }
        public virtual ICollection<Users> Users { get; set; }
        public virtual ICollection<Places> Places { get; set; }
    }
}
