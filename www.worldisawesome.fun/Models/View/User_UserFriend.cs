
namespace www.worldisawesome.fun.ViewModels
{
    public class User_UserFriend
    {
        public string UserId { get; set; }
        public string UserFriendId { get; set; }
        public Models.UserFriendStatusEnum StatusEnum { get; set; }
        public double? RequestedDateTime { get; set; }
        public double? AcceptedDateTime { get; set; }
        public bool? IsConfirmable { get; set; }
    }
}
