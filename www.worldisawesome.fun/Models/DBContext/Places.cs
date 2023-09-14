using System;
using System.Collections.Generic;

namespace www.worldisawesome.fun.DataModels
{
    public partial class Places
    {
        public Places()
        {
            Experiences = new HashSet<Experiences>();
            Users = new HashSet<Users>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Guid? ProfilePictureFileId { get; set; }

        public virtual Files ProfilePictureFile { get; set; }
        public virtual ICollection<Experiences> Experiences { get; set; }
        public virtual ICollection<Users> Users { get; set; }
    }
}
