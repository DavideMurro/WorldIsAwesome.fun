using System;
using www.worldisawesome.fun.Models;

namespace www.worldisawesome.fun.DataModels
{
    public class View_ExperienceFiles
    {
        public Guid ExperienceId { get; set; }
        public string ExperienceName { get; set; }
        public MorningNightEnum? ExperienceMorningNightEnum { get; set; }
        public PrivacyLevelEnum? ExperiencePrivacyLevel { get; set; }
        public ExperienceStatusEnum ExperienceStatusEnum { get; set; }
        public Guid UserId { get; set; }
        public Guid? PlaceId { get; set; }
        public Guid? FileId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public Guid? UserToDo { get; set; }

    }
}
