using System;

namespace www.worldisawesome.fun.DataModels
{
    public partial class Users_ExperiencesToDo
    {
        public Guid UserId { get; set; }
        public Guid ExperienceId { get; set; }
        public virtual Users User { get; set; }
        public virtual Experiences ExperienceToDo { get; set; }
    }
}
