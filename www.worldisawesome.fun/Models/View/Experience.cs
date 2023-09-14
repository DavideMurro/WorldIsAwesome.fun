using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using www.worldisawesome.fun.Models;

namespace www.worldisawesome.fun.ViewModels
{
    public class Experience
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public MorningNightEnum? MorningNightEnum { get; set; }
        public double DateTimeCreated { get; set; }
        public PrivacyLevelEnum? PrivacyLevel { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public IFormFile File { get; set; }

        // public Place Place { get; set; }
        public string PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string PlaceDescription { get; set; }
        public decimal PlaceLatitude { get; set; }
        public decimal PlaceLongitude { get; set; }
        public string UserId { get; set; }
        public string UserNickname { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserProfilePhotoFileId { get; set; }
        public List<User> UsersToDoList { get; set; }
        public ExperienceStatusEnum StatusEnum { get; set; }
        public bool? IsMine { get; set; }
        public bool? IsToDo { get; set; }
    }
}
