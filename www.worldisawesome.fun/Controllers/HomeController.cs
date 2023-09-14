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
using www.worldisawesome.fun.Models;
using www.worldisawesome.fun.Services;

namespace www.worldisawesome.fun.Controllers
{
    [Route("{action=Index}/{id?}")]
    public class HomeController : Controller
    {
        private readonly WorldIsAwesomeContext _dbContext;
        private readonly IConfiguration _configuration;

        private readonly string Folder_ExperiencesMedia;
        private readonly string Folder_UsersPictures;
        private readonly string Folder_PlacesPictures;

        bool isUserLogged = false;
        Guid? userLoggedIdGuid = null;


        public HomeController(
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

        public async Task<IActionResult> Index(Guid? userId)
        {
            try
            {
                await SetSessionParams();

                var isUserSet = userId != Guid.Empty && userId != null;

                ViewBag.HeaderTitle = isUserSet ? "World profile" : "Home";
                ViewBag.IsMine = ViewBag.IsUserLogged && isUserSet && ViewBag.UserLoggedIdGuid == userId;
                ViewBag.IsHome = !isUserSet;
                ViewBag.IsSearchable = true;
                ViewBag.IsParallax = isUserSet;

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

                    ViewBag.IsAnyExperiences = _dbContext.Experiences.Any(x => x.UserId == userId && x.StatusEnum == ExperienceStatusEnum.Completed);
                }

                ViewBag.IsUserSet = isUserSet;

                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<IActionResult> ContactUs()
        {
            try
            {
                await SetSessionParams();
                ViewBag.HeaderTitle = "Contact us";
                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public IActionResult PrivacyPolicy()
        {
            try
            {
                ViewBag.HeaderTitle = "Privacy policy";

                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IActionResult> Login(string redirect)
        {
            try
            {
                await SetSessionParams();

                ViewBag.HeaderTitle = "Login";
                ViewBag.IsLogin = true;

                if (ViewBag.IsUserLogged)
                {
                    Guid userLoggedIdGuid = ViewBag.UserLoggedIdGuid;
                    var userLogged = await _dbContext.Users
                            .Where(x => x.Id == userLoggedIdGuid && x.StatusEnum == Models.UserStatusEnum.Activated)
                            .FirstOrDefaultAsync();

                    if (userLogged != null && redirect != null)
                    {
                        return Redirect(redirect);
                    }
                    else if (userLogged != null)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<IActionResult> SignUp()
        {
            try
            {
                ViewBag.HeaderTitle = "Sign up";
                await SetSessionParams();

                ViewBag.IsUpdate = false;

                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [SessionAuthorize]
        public async Task<IActionResult> EditUser(string redirect)
        {
            try
            {
                await SetSessionParams();

                ViewBag.HeaderTitle = "Edit user";
                ViewBag.IsUpdate = true;
                ViewBag.IsMine = true;
                ViewBag.Redirect = redirect;

                ViewBag.User = ViewBag.UserLogged;

                ViewBag.IsSetProfilePhoto = ViewBag.User.ProfilePhotoFileId != null;
                ViewBag.IsSetResidence = ViewBag.User.ResidencePlaceId != null;


                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [SessionAuthorize]
        public async Task<IActionResult> InsertExperience(MorningNightEnum morningNightEnum, Guid? placeId, string redirect)
        {
            try
            {
                await SetSessionParams();

                ViewBag.HeaderTitle = "Insert experience";
                ViewBag.Redirect = redirect;
                ViewBag.MorningNightEnum = morningNightEnum;
                ViewBag.IsUpdate = false;
                ViewBag.IsMine = true;
                ViewBag.IsSetPlace = (placeId != null && placeId != Guid.Empty);

                if (ViewBag.IsSetPlace)
                {
                    ViewBag.Experience = await _dbContext.Places
                        .Where(x => x.Id == placeId)
                        .Select(x => new ViewModels.Experience()
                        {
                            PlaceId = x.Id.ToString(),
                            PlaceName = x.Name,
                            PlaceDescription = x.Description,
                            PlaceLatitude = x.Latitude,
                            PlaceLongitude = x.Longitude
                        }).FirstOrDefaultAsync();

                    if (ViewBag.Experience == null) throw new MyException("Place doesn t exist");
                }

                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [SessionAuthorize]
        public async Task<IActionResult> EditExperience(Guid experienceId, string redirect)
        {
            try
            {
                await SetSessionParams();

                ViewBag.HeaderTitle = "Edit experience";
                ViewBag.Redirect = redirect;
                ViewBag.IsUpdate = true;
                ViewBag.IsMine = true;

                ViewBag.Experience = await _dbContext.Experiences
                    .Where(x => x.Id == experienceId && x.UserId == userLoggedIdGuid)
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
                        MorningNightEnum = x.MorningNightEnum,
                        PrivacyLevel = x.PrivacyLevel,
                        FileId = x.MediaFile.Id.ToString(),
                        FileName = x.MediaFile.FileName,
                        FileType = x.MediaFile.FileType,
                        // File = x.MediaFile,
                        PlaceId = x.Place.Id.ToString(),
                        PlaceName = x.Place.Name,
                        PlaceDescription = x.Place.Description,
                        PlaceLatitude = x.Place.Latitude,
                        PlaceLongitude = x.Place.Longitude
                    }).FirstOrDefaultAsync();

                if (ViewBag.Experience == null) throw new MyException("Experience doesn t exist");

                ViewBag.IsSetMediaFile = ViewBag.Experience.FileId != null;
                ViewBag.IsSetPlace = ViewBag.Experience.PlaceId != null;
                ViewBag.IsSetMorningNight = ViewBag.Experience.MorningNightEnum != null;
                ViewBag.IsSetPrivacyLevel = ViewBag.Experience.PrivacyLevel != null;


                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
