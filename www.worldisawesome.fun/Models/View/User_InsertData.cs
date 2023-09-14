using Microsoft.AspNetCore.Http;
using System;

namespace www.worldisawesome.fun.ViewModels
{
    public class User_InsertData
    {
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Description { get; set; }
        public string BirthDate { get; set; }
        public IFormFile ProfilePhotoFile { get; set; }
        public string ResidencePlaceName { get; set; }
        public string ResidencePlaceDescription { get; set; }
        public decimal ResidencePlaceLatitude { get; set; }
        public decimal ResidencePlaceLongitude { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
    }
}
