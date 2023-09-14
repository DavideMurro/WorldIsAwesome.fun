using System.Collections.Generic;
using www.worldisawesome.fun.Models;

namespace www.worldisawesome.fun.ViewModels
{
    public class User
    {
        public string Id { get; set; }
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Description { get; set; }
        public string BirthDate { get; set; }
        public string ProfilePhotoFileId { get; set; }
        public string ProfilePhotoFileName {get; set; }
        public string ProfilePhotoFileType { get; set; }

        public string Mail { get; set; }
        public double? LastAccess { get; set; }
        public string ResidencePlaceId { get; set; }
        public string ResidencePlaceName { get; set; }
        public string ResidencePlaceDescription { get; set; }
        public decimal? ResidencePlaceLatitude { get; set; }
        public decimal? ResidencePlaceLongitude { get; set; }

        public UserStatusEnum StatusEnum { get; set; }
        public UserFriendStatusEnum? FriendStatusEnum { get; set; }
        public string FriendRequesterUserId { get; set; }
        public string FriendRecieverUserId { get; set; }
        public double? FriendRequestedDateTime { get; set; }
        public double? FriendAcceptedDateTime { get; set; }
        public bool? FriendIsConfirmable { get; set; }

        public List<User> FriendList { get; set; }
        public bool? IsMine { get; set; }
        public bool? IsFriend { get; set; }
    }
}
