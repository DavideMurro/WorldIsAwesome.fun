using System;
using www.worldisawesome.fun.Models;

namespace www.worldisawesome.fun.DataModels
{
    public class View_PlaceExperiences
    {
        public Guid PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string PlaceDescription { get; set; }
        public decimal PlaceLatitude { get; set; }
        public decimal PlaceLongitude { get; set; }
        public Guid? PlaceProfilePictureFileId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ExperienceId { get; set; }
        public string ExperienceName { get; set; }
        public string ExperienceDescription { get; set; }
        public DateTime? ExperienceDate { get; set; }
        public TimeSpan? ExperienceTime { get; set; }
        public DateTimeOffset ExperienceDateTimeCreated { get; set; }
        public MorningNightEnum? ExperienceMorningNightEnum { get; set; }
        public int ExperienceMediaFileId { get; set; }
        public PrivacyLevelEnum? ExperiencePrivacyLevel { get; set; }
        public ExperienceStatusEnum ExperienceStatusEnum { get; set; }
        public Guid? UserToDo { get; set; }
    }
}
