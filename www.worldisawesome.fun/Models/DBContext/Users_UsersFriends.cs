using System;

namespace www.worldisawesome.fun.DataModels
{
    public partial class Users_UsersFriends
    {
        public Guid UserId { get; set; }
        public Guid UserFriendId { get; set; }
        public Models.UserFriendStatusEnum StatusEnum { get; set; }
        public DateTimeOffset RequestedDateTime { get; set; }
        public DateTimeOffset? AcceptedDateTime { get; set; }

        public virtual Users User { get; set; }
        public virtual Users UserFriend { get; set; }
    }
}
