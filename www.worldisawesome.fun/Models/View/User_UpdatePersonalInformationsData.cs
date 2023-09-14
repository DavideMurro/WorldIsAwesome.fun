using Microsoft.AspNetCore.Http;
using System;

namespace www.worldisawesome.fun.ViewModels
{
    public class User_UpdatePersonalInformationsData
    {
        public string UserId { get; set; }
        // public string Nickname { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Description { get; set; }
        public string BirthDate { get; set; }
        public string ProfilePhotoFileId { get; set; }
        public IFormFile ProfilePhotoFile { get; set; }
        public string ResidencePlaceId { get; set; }
        public string ResidencePlaceName { get; set; }
        public string ResidencePlaceDescription { get; set; }
        public decimal ResidencePlaceLatitude { get; set; }
        public decimal ResidencePlaceLongitude { get; set; }
    }
}
