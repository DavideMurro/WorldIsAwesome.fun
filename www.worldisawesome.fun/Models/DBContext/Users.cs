using System;
using System.Collections.Generic;
using www.worldisawesome.fun.Models;

namespace www.worldisawesome.fun.DataModels
{
    public partial class Users
    {
        public Users()
        {
            Experiences = new HashSet<Experiences>();
            ExperiencesToDo = new HashSet<Users_ExperiencesToDo>();
            UsersFriends = new HashSet<Users_UsersFriends>();
        }

        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public Guid? ProfilePhotoFileId { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public DateTimeOffset? LastAccess { get; set; }
        public Guid? ResidencePlaceId { get; set; }
        public UserStatusEnum StatusEnum { get; set; }
        public DateTimeOffset RegistrationDateTime { get; set; }

        public virtual Files ProfilePhotoFile { get; set; }
        public virtual Places ResidencePlace { get; set; }
        public virtual ICollection<Experiences> Experiences { get; set; }
        public virtual ICollection<Users_ExperiencesToDo> ExperiencesToDo { get; set; }
        public virtual ICollection<Users_UsersFriends> UsersFriends { get; set; }
    }
}
