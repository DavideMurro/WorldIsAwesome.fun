using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using www.worldisawesome.fun.DBContext;
using www.worldisawesome.fun.ExceptionModels;
using www.worldisawesome.fun.Services;

namespace www.worldisawesome.fun.Controllers
{
    [Route("{controller=Home}/{action=Index}/{id?}")]
    public class SearchController : Controller
    {
        private readonly WorldIsAwesomeContext _dbContext;
        private readonly IConfiguration _configuration;

        private readonly string Folder_ExperiencesMedia;
        private readonly string Folder_UsersPictures;
        private readonly string Folder_PlacesPictures;

        bool isUserLogged = false;
        Guid? userLoggedIdGuid = null;


        public SearchController(
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

        public async Task<IActionResult> Experiences(Guid? userId)
        {
            try
            {
                await SetSessionParams();

                var isUserSet = userId != Guid.Empty && userId != null;

                ViewBag.HeaderTitle = "Search experiences";
                ViewBag.IsUserSet = isUserSet;
                ViewBag.IsMine = ViewBag.IsUserLogged && userId == ViewBag.UserLoggedIdGuid;

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

                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<IActionResult> People(Guid? userId, Models.ViewTypeEnum viewTypeEnum, Guid? experienceId)
        {
            try
            {
                await SetSessionParams();

                var isUserSet = userId != Guid.Empty && userId != null;
                var isExperienceSet = experienceId != Guid.Empty && experienceId != null && viewTypeEnum == Models.ViewTypeEnum.ExperiencesToDo;

                ViewBag.HeaderTitle = "Search people";
                ViewBag.IsUserSet = isUserSet;
                ViewBag.IsMine = ViewBag.IsUserLogged && userId == ViewBag.UserLoggedIdGuid;
                ViewBag.IsExperienceSet = isExperienceSet;

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
                if (isExperienceSet)
                {
                    ViewBag.Experience = await _dbContext
                        .Experiences
                        .Where(x => x.Id == experienceId)
                        .Select(x => new ViewModels.Experience()
                        {
                            Id = x.Id.ToString(),
                            Name = x.Name,
                            Description = x.Description,
                            Date = DateTimeHelpers.GetDateFormattedFromDateTime(x.Date),
                            Time = DateTimeHelpers.GetTimeFormattedFromTimeSpan(x.Time),
                            MorningNightEnum = x.MorningNightEnum,
                            PrivacyLevel = x.PrivacyLevel,
                            FileId = x.MediaFile.Id.ToString(),
                            PlaceId = x.Place.Id.ToString(),
                            UserId = x.User.Id.ToString()
                        }).FirstOrDefaultAsync();

                    if (ViewBag.Experience == null) throw new MyException("Experience not found");

                    ViewBag.IsMine = isUserLogged && ViewBag.Experience.UserId.Equals(ViewBag.UserLogged.Id, StringComparison.OrdinalIgnoreCase);
                }

                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<IActionResult> Places(Guid? userId)
        {
            try
            {
                await SetSessionParams();

                var isUserSet = userId != Guid.Empty && userId != null;

                ViewBag.HeaderTitle = "Search places";
                ViewBag.IsUserSet = isUserSet;
                ViewBag.IsMine = ViewBag.IsUserLogged && userId == ViewBag.UserLoggedIdGuid;

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

                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
