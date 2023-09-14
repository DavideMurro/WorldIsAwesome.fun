using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using www.worldisawesome.fun.DBContext;
using www.worldisawesome.fun.ExceptionModels;
using www.worldisawesome.fun.Models;
using www.worldisawesome.fun.Services;

namespace www.worldisawesome.fun.Controllers
{
    [Route("{controller=Home}/{action=Index}/{id?}")]
    public class ViewController : Controller
    {
        private readonly WorldIsAwesomeContext _dbContext;
        private readonly IConfiguration _configuration;

        private readonly string Folder_ExperiencesMedia;
        private readonly string Folder_UsersPictures;
        private readonly string Folder_PlacesPictures;

        bool isUserLogged = false;
        Guid? userLoggedIdGuid = null;


        public ViewController(
            WorldIsAwesomeContext dbContext,
            IConfiguration configuration
        )
        {
            _dbContext = dbContext;
            _configuration = configuration;

            Folder_ExperiencesMedia = _configuration.GetValue<string>("FolderManagement:ExperiencesMedia");
            Folder_UsersPictures = _configuration.GetValue<string>("FolderManagement:UsersPictures");
            Folder_PlacesPictures = _configuration.GetValue<string>("FolderManagement:PlacesPictures");
        }

        private async Task SetSessionParams()
        {
            var userLoggedId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userLoggedId != null && userLoggedId != default)
            {
                var userLogged = await _dbContext.Users
                    .Where(x => x.Id == Guid.Parse(userLoggedId) && x.StatusEnum == Models.UserStatusEnum.Activated)
                    .Include(x => x.ResidencePlace)
                    .FirstOrDefaultAsync();

                if (userLogged == null)
                {
                    isUserLogged = false;
                    userLoggedIdGuid = null;
                    ViewBag.UserLogged = null;
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    //throw new MyUnauthorizedException("User set is wrong or not active");
                }
                else
                {
                    userLogged.LastAccess = DateTimeOffset.Now;
                    _dbContext.Users.Update(userLogged);
                    await _dbContext.SaveChangesAsync();

                    isUserLogged = true;
                    userLoggedIdGuid = userLogged.Id;
                    ViewBag.UserLogged = new ViewModels.User()
                    {
                        Id = userLogged.Id.ToString(),
                        Nickname = userLogged.Nickname,
                        Name = userLogged.Name,
                        Surname = userLogged.Surname,
                        Description = userLogged.Description,
                        BirthDate = DateTimeHelpers.GetDateFormattedFromDateTime(userLogged.BirthDate),
                        ProfilePhotoFileId = userLogged.ProfilePhotoFileId.ToString(),
                        Mail = userLogged.Mail,
                        LastAccess = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(userLogged.LastAccess),
                        ResidencePlaceId = userLogged.ResidencePlace?.Id.ToString(),
                        ResidencePlaceName = userLogged.ResidencePlace?.Name
                    };
                }
            }

            ViewBag.IsUserLogged = isUserLogged;
            ViewBag.UserLoggedIdGuid = userLoggedIdGuid;
        }


        #region pages
        public async Task<IActionResult> Experience(Guid experienceId)
        {
            try
            {
                await SetSessionParams();

                ViewBag.HeaderTitle = "Experience";
                ViewBag.Experience = await _dbContext.Experiences
                    .Where(x => x.Id == experienceId &&
                        ((isUserLogged && x.UserId == userLoggedIdGuid) || x.StatusEnum == Models.ExperienceStatusEnum.Completed) && 
                        ((isUserLogged && x.UserId == userLoggedIdGuid) || (isUserLogged && x.PrivacyLevel == PrivacyLevelEnum.Confidential && _dbContext.Users_UsersFriends.Any(uf => ((uf.UserId == userLoggedIdGuid && uf.UserFriendId == x.UserId) || (uf.UserId == x.UserId && uf.UserFriendId == userLoggedIdGuid)) && uf.StatusEnum == UserFriendStatusEnum.Accepted)) || x.PrivacyLevel == PrivacyLevelEnum.Public))
                    .Include(x => x.User)
                    .Include(x => x.Place)
                    .Include(x => x.UsersToDo)
                    .Include(x => x.MediaFile)
                    .Select(x => new ViewModels.Experience()
                    {
                        Id = x.Id.ToString(),
                        Name = x.Name,
                        Description = x.Description,
                        Date = DateTimeHelpers.GetDateFormattedFromDateTime(x.Date),
                        Time = DateTimeHelpers.GetTimeFormattedFromTimeSpan(x.Time.Value),
                        PrivacyLevel = x.PrivacyLevel,
                        MorningNightEnum = x.MorningNightEnum,
                        StatusEnum = x.StatusEnum,
                        FileId = x.MediaFile.Id.ToString(),
                        FileName = x.MediaFile.FileName,
                        FileType = x.MediaFile.FileType,
                        UserId = x.User.Id.ToString(),
                        UserName = x.User.Name,
                        UserSurname = x.User.Surname,
                        UserNickname = x.User.Nickname,
                        UserProfilePhotoFileId = x.User.ProfilePhotoFileId.ToString(),
                        PlaceId = x.Place.Id.ToString(),
                        PlaceName = x.Place.Name,
                        UsersToDoList = x.UsersToDo.Select(x => new ViewModels.User()
                        {
                            Id = x.User.Id.ToString(),
                            Nickname = x.User.Nickname,
                            Name = x.User.Name,
                            Surname = x.User.Surname
                        }).ToList()
                    }).FirstOrDefaultAsync();

                if (ViewBag.Experience == null) throw new MyException("Experience not found");

                ViewBag.IsMine = ViewBag.IsUserLogged && ViewBag.Experience.UserId.Equals(ViewBag.UserLogged.Id, StringComparison.OrdinalIgnoreCase);
                ViewBag.IsToDo = ViewBag.IsUserLogged && ViewBag.Experience.UsersToDoList != null && ((ViewBag.Experience.UsersToDoList as IEnumerable<ViewModels.User>).FirstOrDefault(x => x.Id.Equals(ViewBag.UserLogged.Id, StringComparison.OrdinalIgnoreCase)) != null);

                //ViewBag.FileStream = await GetMediaStream(ViewBag.Experience.FileId);

                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IActionResult> Place(Guid placeId, Models.MorningNightEnum morningNightEnum, Models.ViewTypeEnum viewTypeEnum, System.Guid? userId)
        {
            try
            {
                await SetSessionParams();

                // var maxLength_description = 100;
                var isUserSet = userId != null && userId != Guid.Empty;
                var isMine = ViewBag.IsUserLogged && isUserSet && userId == ViewBag.UserLoggedIdGuid;

                ViewBag.HeaderTitle = "Place";

                if ((viewTypeEnum == Models.ViewTypeEnum.MyExperiences || viewTypeEnum == Models.ViewTypeEnum.ExperiencesToDo) && userId == null)
                {
                    throw new MyException("User not set");
                } else if (userId != null && viewTypeEnum == ViewTypeEnum.Default)
                {
                    viewTypeEnum = ViewTypeEnum.MyExperiences;
                    // throw new MyException("ViewType not set");
                }

                ViewBag.IsMine = isMine;
                ViewBag.IsUserSet = isUserSet;

                ViewBag.MorningNightEnum = morningNightEnum;

                ViewBag.ViewTypeEnum = viewTypeEnum;

                if (isUserSet)
                {
                    ViewBag.User = await _dbContext.Users
                            .Where(x => x.Id == userId)
                            .Select(x => new ViewModels.User()
                            {
                                Id = x.Id.ToString(),
                                Nickname = x.Nickname,
                                Name = x.Name,
                                Surname = x.Surname,
                                Description = x.Description,
                                ProfilePhotoFileId = x.ProfilePhotoFile.Id.ToString(),
                                ProfilePhotoFileName = x.ProfilePhotoFile.FileName,
                                ProfilePhotoFileType = x.ProfilePhotoFile.FileType
                            }).FirstOrDefaultAsync();
                }

                ViewBag.Place = await _dbContext
                        .View_PlaceExperiences
                        .Where(x => x.PlaceId == placeId)
                        .GroupBy(x => new { x.PlaceId, x.PlaceName, x.PlaceDescription, x.PlaceLatitude, x.PlaceLongitude, x.PlaceProfilePictureFileId })
                        .OrderBy(x => x.Key.PlaceName)
                        .Select(x => new ViewModels.Place()
                        {
                            Id = x.Key.PlaceId.ToString(),
                            Name = x.Key.PlaceName,
                            Description = x.Key.PlaceDescription,
                            Latitude = x.Key.PlaceLatitude,
                            Longitude = x.Key.PlaceLongitude,
                            PictureId = x.Key.PlaceProfilePictureFileId.ToString()
                        }).FirstOrDefaultAsync();

                if (ViewBag.Place == null) throw new MyException("Place not found");

                ViewBag.ExperienceList = await _dbContext.Experiences
                    .Where(x => x.PlaceId == placeId &&
                        (userId == null || viewTypeEnum == Models.ViewTypeEnum.ExperiencesToDo || x.UserId == userId) &&
                        (morningNightEnum == Models.MorningNightEnum.Default || x.MorningNightEnum == morningNightEnum) &&
                        ((isUserLogged && x.UserId == userLoggedIdGuid) || x.StatusEnum == Models.ExperienceStatusEnum.Completed) &&
                        ((isUserLogged && x.UserId == userLoggedIdGuid && viewTypeEnum != Models.ViewTypeEnum.ExperiencesToDo) || (isUserLogged && x.PrivacyLevel == PrivacyLevelEnum.Confidential && _dbContext.Users_UsersFriends.Any(uf => ((uf.UserId == userLoggedIdGuid && uf.UserFriendId == x.UserId) || (uf.UserId == x.UserId && uf.UserFriendId == userLoggedIdGuid)) && uf.StatusEnum == UserFriendStatusEnum.Accepted)) || x.PrivacyLevel == PrivacyLevelEnum.Public) &&
                        (viewTypeEnum != Models.ViewTypeEnum.ExperiencesToDo || _dbContext.Users_ExperiencesToDo.Any(ue => ue.UserId == userId && ue.ExperienceId == x.Id))
                     )
                    .Include(x => x.User)
                    .Include(x => x.MediaFile)
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(x => x.Time)
                    .Select(x => new ViewModels.Experience()
                    {
                        Id = x.Id.ToString(),
                        Name = x.Name,
                        //Description = x.Description.Length <= maxLength_description ? x.Description : x.Description.Substring(0, maxLength_description) + "...",
                        Description = x.Description,
                        Date = DateTimeHelpers.GetDateFormattedFromDateTime(x.Date),
                        Time = DateTimeHelpers.GetTimeFormattedFromTimeSpan(x.Time.Value),
                        PrivacyLevel = x.PrivacyLevel,
                        MorningNightEnum = x.MorningNightEnum,
                        StatusEnum = x.StatusEnum,
                        FileId = x.MediaFile.Id.ToString(),
                        FileName = x.MediaFile.FileName,
                        FileType = x.MediaFile.FileType,
                        UserId = x.User.Id.ToString(),
                        UserName = x.User.Name,
                        UserSurname = x.User.Surname,
                        UserNickname = x.User.Nickname,
                        UserProfilePhotoFileId = x.User.ProfilePhotoFileId.ToString(),
                        PlaceId = x.Place.Id.ToString(),
                        PlaceName = x.Place.Name,
                        IsMine = isUserLogged && x.UserId == userLoggedIdGuid,
                        IsToDo = false  // TODO: fare isTodo
                    }).ToListAsync();

                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public new async Task<IActionResult> User(Guid userId, Models.MorningNightEnum morningNightEnum, Models.ViewTypeEnum viewTypeEnum)
        {
            try
            {
                await SetSessionParams();

                // var maxLength_description = 100;
                var isMine = ViewBag.IsUserLogged && userId == ViewBag.UserLoggedIdGuid;
                var isUserSet = userId != Guid.Empty;

                ViewBag.HeaderTitle = "Profile";

                if (!isUserSet) throw new MyException("User not set");

                if (viewTypeEnum == ViewTypeEnum.Default)
                {
                    viewTypeEnum = ViewTypeEnum.MyExperiences;
                    // throw new MyException("ViewType not set");
                }

                ViewBag.IsMine = isMine;
                ViewBag.IsUserSet = isUserSet;

                ViewBag.MorningNightEnum = morningNightEnum;

                ViewBag.ViewTypeEnum = viewTypeEnum;


                if (isUserSet)
                {
                    ViewBag.User = await _dbContext
                        .Users
                        .Where(x => x.Id == userId)
                        .Include(x => x.ProfilePhotoFile)
                        .Include(x => x.UsersFriends)
                        .Select(x => new ViewModels.User()
                        {
                            Id = x.Id.ToString(),
                            Nickname = x.Nickname,
                            Name = x.Name,
                            Surname = x.Surname,
                            Description = x.Description,
                            BirthDate = DateTimeHelpers.GetDateFormattedFromDateTime(x.BirthDate),
                            ProfilePhotoFileId = x.ProfilePhotoFile.Id.ToString(),
                            ProfilePhotoFileName = x.ProfilePhotoFile.FileName,
                            ProfilePhotoFileType = x.ProfilePhotoFile.FileType,
                            ResidencePlaceId = x.ResidencePlace.Id.ToString(),
                            ResidencePlaceName = x.ResidencePlace.Name,
                            StatusEnum = x.StatusEnum
                        }).FirstOrDefaultAsync();
                    if (ViewBag.User == null) throw new MyException("User not found");
                }


                ViewBag.ExperienceList = await _dbContext.Experiences
                    .Where(x => (viewTypeEnum == Models.ViewTypeEnum.ExperiencesToDo || x.UserId == userId) &&
                        (morningNightEnum == Models.MorningNightEnum.Default || x.MorningNightEnum == morningNightEnum) &&
                        (x.StatusEnum == Models.ExperienceStatusEnum.Completed) &&
                        ((isUserLogged && x.UserId == userLoggedIdGuid && viewTypeEnum != Models.ViewTypeEnum.ExperiencesToDo) || (isUserLogged && x.PrivacyLevel == PrivacyLevelEnum.Confidential && _dbContext.Users_UsersFriends.Any(uf => ((uf.UserId == userLoggedIdGuid && uf.UserFriendId == x.UserId) || (uf.UserId == x.UserId && uf.UserFriendId == userLoggedIdGuid)) && uf.StatusEnum == UserFriendStatusEnum.Accepted)) || x.PrivacyLevel == PrivacyLevelEnum.Public) &&
                        (viewTypeEnum != Models.ViewTypeEnum.ExperiencesToDo || _dbContext.Users_ExperiencesToDo.Any(ue => ue.UserId == userId && ue.ExperienceId == x.Id)))
                    .OrderBy(x => Guid.NewGuid())
                    .Take(5)
                    .Include(x => x.User)
                    .Include(x => x.MediaFile)
                    .Select(x => new ViewModels.Experience()
                    {
                        Id = x.Id.ToString(),
                        Name = x.Name,
                        //Description = x.Description.Length <= maxLength_description ? x.Description : x.Description.Substring(0, maxLength_description) + "...",
                        Description = x.Description,
                        Date = DateTimeHelpers.GetDateFormattedFromDateTime(x.Date),
                        Time = DateTimeHelpers.GetTimeFormattedFromTimeSpan(x.Time.Value),
                        PrivacyLevel = x.PrivacyLevel,
                        MorningNightEnum = x.MorningNightEnum,
                        StatusEnum = x.StatusEnum,
                        FileId = x.MediaFile.Id.ToString(),
                        FileName = x.MediaFile.FileName,
                        FileType = x.MediaFile.FileType,
                        UserId = x.User.Id.ToString(),
                        UserName = x.User.Name,
                        UserSurname = x.User.Surname,
                        UserNickname = x.User.Nickname,
                        UserProfilePhotoFileId = x.User.ProfilePhotoFileId.ToString(),
                        PlaceId = x.Place.Id.ToString(),
                        PlaceName = x.Place.Name,
                        IsMine = isUserLogged && x.UserId == userLoggedIdGuid,
                        IsToDo = false  // TODO: fare isTodo
                    }).ToListAsync();


                ViewBag.PlaceList = await _dbContext.View_PlaceExperiences
                        .Where(x => (viewTypeEnum == Models.ViewTypeEnum.ExperiencesToDo || x.UserId == userId) && (viewTypeEnum != Models.ViewTypeEnum.ExperiencesToDo || x.UserToDo == userId) && (morningNightEnum == Models.MorningNightEnum.Default || x.ExperienceMorningNightEnum == morningNightEnum) /*&& (EF.Functions.Like(x.PlaceName, "%" + searchText + "%"))*/)
                        .GroupBy(x => new { x.PlaceId, x.PlaceName, x.PlaceDescription, x.PlaceLatitude, x.PlaceLongitude, x.PlaceProfilePictureFileId/*, x.UserId*/ })
                        .OrderBy(x => Guid.NewGuid())
                        .Take(3)
                        .Select(x => new ViewModels.Place()
                        {
                            Id = x.Key.PlaceId.ToString(),
                            Name = x.Key.PlaceName,
                            //PlaceDescription = x.Key.PlaceDescription.Length <= maxLength_description ? x.Key.PlaceDescription : x.Key.PlaceDescription.Substring(0, maxLength_description) + "...",
                            Description = x.Key.PlaceDescription,
                            Latitude = x.Key.PlaceLatitude,
                            Longitude = x.Key.PlaceLongitude,
                            PictureId = x.Key.PlaceProfilePictureFileId.ToString()
                            // Experience_Count = x.Count()
                        }).ToListAsync();


                ViewBag.FriendList = await _dbContext.Users_UsersFriends
                    .Where(x => (x.UserId == userId || x.UserFriendId == userId) && x.StatusEnum == Models.UserFriendStatusEnum.Accepted /*&& (EF.Functions.Like(x.PlaceName, "%" + searchText + "%"))*/)
                    .OrderBy(x => Guid.NewGuid())
                    .Take(6)
                    .Select(x => new ViewModels.User()
                    {
                        Id = x.UserId == userId ? x.UserFriend.Id.ToString() : x.User.Id.ToString(),
                        Nickname = x.UserId == userId ? x.UserFriend.Nickname : x.User.Nickname,
                        Name = x.UserId == userId ? x.UserFriend.Name : x.User.Name,
                        Surname = x.UserId == userId ? x.UserFriend.Surname : x.User.Surname,
                        Description = x.UserId == userId ? x.UserFriend.Description : x.User.Description,
                        BirthDate = x.UserId == userId ? DateTimeHelpers.GetDateFormattedFromDateTime(x.UserFriend.BirthDate) : DateTimeHelpers.GetDateFormattedFromDateTime(x.User.BirthDate),
                        ProfilePhotoFileId = x.UserId == userId ? x.UserFriend.ProfilePhotoFile.Id.ToString() : x.User.ProfilePhotoFile.Id.ToString(),
                        ProfilePhotoFileName = x.UserId == userId ? x.UserFriend.ProfilePhotoFile.FileName : x.User.ProfilePhotoFile.FileName,
                        ProfilePhotoFileType = x.UserId == userId ? x.UserFriend.ProfilePhotoFile.FileType : x.User.ProfilePhotoFile.FileType,
                        ResidencePlaceId = x.UserId == userId ? x.UserFriend.ResidencePlace.Id.ToString() : x.User.ResidencePlace.Id.ToString(),
                        ResidencePlaceName = x.UserId == userId ? x.UserFriend.ResidencePlace.Name : x.User.ResidencePlace.Name,
                        StatusEnum = x.UserId == userId ? x.UserFriend.StatusEnum : x.User.StatusEnum,
                        FriendStatusEnum = x.StatusEnum,
                        FriendRequesterUserId = x.User.Id.ToString(),
                        FriendRecieverUserId = x.UserFriend.Id.ToString(),
                        FriendRequestedDateTime = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.RequestedDateTime),
                        FriendAcceptedDateTime = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.AcceptedDateTime)
                    })
                    .ToListAsync();

                if (ViewBag.IsUserLogged && ViewBag.FriendList != null)
                {
                    ViewBag.FriendCurrentUser = await _dbContext.Users_UsersFriends
                        .Where(x => (x.UserId == userId && x.UserFriendId == userLoggedIdGuid) || (x.UserId == userLoggedIdGuid && x.UserFriendId == userId))
                        .Select(x => new ViewModels.User_UserFriend()
                        {
                            UserId = x.UserId.ToString(),
                            UserFriendId = x.UserFriendId.ToString(),
                            StatusEnum = x.StatusEnum,
                            RequestedDateTime = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.RequestedDateTime),
                            AcceptedDateTime = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.AcceptedDateTime),
                            IsConfirmable = x.UserFriendId == userLoggedIdGuid
                        })
                        .FirstOrDefaultAsync();
                }
                else
                {
                    ViewBag.FriendCurrentUser = null;
                }


                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion


        #region get file
        public async Task<FileStreamResult> GetExperienceMediaStream(string mediaId, bool isPreview)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;
                var mediaIdGuid = Guid.Parse(mediaId);

                var media = await _dbContext.View_ExperienceFiles
                    .FirstOrDefaultAsync(x => x.FileId == mediaIdGuid &&
                        ((isUserLogged && x.UserId == userLoggedIdGuid) || x.ExperienceStatusEnum == Models.ExperienceStatusEnum.Completed) &&
                        ((isUserLogged && x.UserId == userLoggedIdGuid) || (isUserLogged && x.ExperiencePrivacyLevel == PrivacyLevelEnum.Confidential && _dbContext.Users_UsersFriends.Any(uf => ((uf.UserId == userLoggedIdGuid && uf.UserFriendId == x.UserId) || (uf.UserId == x.UserId && uf.UserFriendId == userLoggedIdGuid)) && uf.StatusEnum == UserFriendStatusEnum.Accepted)) || x.ExperiencePrivacyLevel == Models.PrivacyLevelEnum.Public)
                        );
                if (media == null)
                    throw new Exception("Media not present");

                string path;
                if (isPreview && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, "Previews", media.FileName)))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, "Previews", media.FileName);
                }
                else if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, media.FileName)))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, media.FileName);
                }
                else
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, "Temp", media.FileName);
                }
                var image = System.IO.File.OpenRead(path);
                return File(image, media.FileType);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<FileStreamResult> GetUserPictureStream(string pictureId, bool isPreview)
        {
            try
            {
                var pictureIdGuid = Guid.Parse(pictureId);

                var picture = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == pictureIdGuid);
                if (picture == null)
                    throw new Exception("Picture not present");

                string path;
                if (isPreview && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, "Previews", picture.FileName)))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, "Previews", picture.FileName);
                }
                else if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, picture.FileName)))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, picture.FileName);
                }
                else
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, "Temp", picture.FileName);
                }
                var image = System.IO.File.OpenRead(path);
                return File(image, picture.FileType);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<FileStreamResult> GetPlacePictureStream(string pictureId, bool isPreview)
        {
            try
            {
                var pictureIdGuid = Guid.Parse(pictureId);

                var picture = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == pictureIdGuid);
                if (picture == null)
                    throw new Exception("Picture not present");

                string path;
                if (isPreview && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_PlacesPictures, "Previews", picture.FileName)))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_PlacesPictures, "Previews", picture.FileName);
                }
                else if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_PlacesPictures, picture.FileName)))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_PlacesPictures, picture.FileName);
                }
                else
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_PlacesPictures, "Temp", picture.FileName);
                }
                var image = System.IO.File.OpenRead(path);
                return File(image, picture.FileType);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<FileStreamResult> GetDefaultPlacePictureStream(string placeId, bool isPreview)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;

                var placeIdGuid = Guid.Parse(placeId);

                var place = await _dbContext.Places.FirstOrDefaultAsync(x => x.Id == placeIdGuid);

                if (place.ProfilePictureFileId != null)
                {
                    return await GetPlacePictureStream(place.ProfilePictureFileId.ToString(), isPreview);
                } else
                {
                    var file = await _dbContext.View_ExperienceFiles
                    .Where(x => x.PlaceId == placeIdGuid &&
                        x.ExperienceStatusEnum == Models.ExperienceStatusEnum.Completed &&
                        x.ExperiencePrivacyLevel == Models.PrivacyLevelEnum.Public &&
                        EF.Functions.Like(x.FileType, "image/%"))
                    .OrderBy(x => Guid.NewGuid())
                    .FirstOrDefaultAsync();

                    if (file != null)
                    {
                        return await GetExperienceMediaStream(file.FileId.ToString(), isPreview);
                    } else
                    {
                        throw new MyException("Media not found");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion


        #region other
        public static string StripHtml(string htmlMarkup)
        {
            try
            {
                return WebUtility.HtmlDecode(Regex.Replace(htmlMarkup, "<[^>]*(>|$)", string.Empty));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        #endregion
    }
}