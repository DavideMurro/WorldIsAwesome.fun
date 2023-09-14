using System;
using System.Collections.Generic;
using www.worldisawesome.fun.Models;

namespace www.worldisawesome.fun.DataModels
{
    public partial class Experiences
    {
        public Experiences()
        {
            UsersToDo = new HashSet<Users_ExperiencesToDo>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public MorningNightEnum? MorningNightEnum { get; set; }
        public Guid? MediaFileId { get; set; }
        public Guid? PlaceId { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset DateTimeCreated { get; set; }
        public PrivacyLevelEnum? PrivacyLevel { get; set; }
        public ExperienceStatusEnum StatusEnum { get; set; }

        public virtual Files MediaFile { get; set; }
        public virtual Places Place { get; set; }
        public virtual Users User { get; set; }

        public virtual ICollection<Users_ExperiencesToDo> UsersToDo { get; set; }
    }
}
