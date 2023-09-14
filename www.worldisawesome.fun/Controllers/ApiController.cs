using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using www.worldisawesome.fun.DBContext;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using www.worldisawesome.fun.Services;
using www.worldisawesome.fun.ExceptionModels;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace www.worldisawesome.fun.Controllers
{
    [ApiController]
    [Route("{controller=Home}/{action=Index}/{id?}")]
    public class ApiController : ControllerBase
    {
        private readonly WorldIsAwesomeContext _dbContext;
        private readonly IConfiguration _configuration;

        private readonly string Folder_ExperiencesMedia;
        private readonly string Folder_UsersPictures;
        private readonly string Folder_PlacesPictures;

        private readonly string SMTPMail_Server;
        private readonly int SMTPMail_Port;
        private readonly string SMTPMail_Username;
        private readonly string SMTPMail_Passwrod;
        private readonly bool SMTPMail_IsSSL;


        public ApiController(
            WorldIsAwesomeContext dbContext,
            IConfiguration configuration
        )
        {
            _dbContext = dbContext;
            _configuration = configuration;

            // FILES FOLDERS
            Folder_ExperiencesMedia = _configuration.GetValue<string>("FolderManagement:ExperiencesMedia");
            Folder_UsersPictures = _configuration.GetValue<string>("FolderManagement:UsersPictures");
            Folder_PlacesPictures = _configuration.GetValue<string>("FolderManagement:PlacesPictures");

            // SMTP
            SMTPMail_Server = _configuration.GetValue<string>("SMTPMail:Server");
            SMTPMail_Port = _configuration.GetValue<int>("SMTPMail:Port");
            SMTPMail_Username = _configuration.GetValue<string>("SMTPMail:Username");
            SMTPMail_Passwrod = _configuration.GetValue<string>("SMTPMail:Password");
            SMTPMail_IsSSL = _configuration.GetValue<bool>("SMTPMail:IsSSL");
        }


        #region LOGIN
        [HttpPost]
        public async Task<ViewModels.User> Login([FromForm] ViewModels.LoginData login)
        {
            try
            {
                var username = login.Username.Trim();
                var password = login.Password.Trim();
                var isRememberMe = login.IsRememberMe;

                var user = await _dbContext.Users
                    .Where(x => x.Mail == username || x.Nickname == username)
                    .Include(x => x.ResidencePlace)
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    if (user.StatusEnum == Models.UserStatusEnum.Activated)
                    {
                        if (SecurePasswordHasher.Verify(password, user.Password))
                        {
                            // update lastAccess
                            user.LastAccess = DateTimeOffset.Now;
                            _dbContext.Users.Update(user);
                            await _dbContext.SaveChangesAsync();


                            // set cookie authentication
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(ClaimTypes.Name, user.Nickname),
                                //new Claim(ClaimTypes.Role, "Administrator")
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var authProperties = new AuthenticationProperties
                            {
                                ExpiresUtc = isRememberMe ? DateTimeOffset.UtcNow.AddYears(1) : DateTimeOffset.UtcNow.AddDays(1),
                                IsPersistent = true
                            };
                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties);


                            // get user view
                            var userView = new ViewModels.User()
                            {
                                Id = user.Id.ToString(),
                                Nickname = user.Nickname,
                                Name = user.Name,
                                Surname = user.Surname,
                                Description = user.Description,
                                BirthDate = DateTimeHelpers.GetDateFormattedFromDateTime(user.BirthDate),
                                ProfilePhotoFileId = user.ProfilePhotoFileId.ToString(),
                                Mail = user.Mail,
                                LastAccess = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(user.LastAccess),
                                ResidencePlaceId = user.ResidencePlace?.Id.ToString(),
                                ResidencePlaceName = user.ResidencePlace?.Name
                            };

                            // return
                            return userView;
                        }
                        else
                        {
                            throw new MyException("Password of User is wrong");
                        }
                    }
                    else if (user.StatusEnum == Models.UserStatusEnum.Default)
                    {
                        throw new MyException("User not activated! Please, check your mail");
                    }
                    else if (user.StatusEnum == Models.UserStatusEnum.Banned)
                    {
                        throw new MyException("This user is banned! please contact us for more informations");
                    }
                    else
                    {
                        throw new MyException("This user is not activated");
                    }
                }
                else
                {
                    throw new MyException("No User Found, check the username");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpPost]
        public async Task<bool> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<ViewModels.User> CheckLogin()
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    throw new MyException("Session not set");
                }
                else
                {
                    var userIdGuid = Guid.Parse(userId);
                    var user = await _dbContext.Users
                        .Where(x => x.Id == userIdGuid && x.StatusEnum == Models.UserStatusEnum.Activated)
                        .Include(x => x.ResidencePlace)
                        .FirstOrDefaultAsync();

                    if (user != null)
                    {
                        user.LastAccess = DateTimeOffset.Now;
                        _dbContext.Users.Update(user);
                        await _dbContext.SaveChangesAsync();

                        // get user view
                        var userView = new ViewModels.User()
                        {
                            Id = user.Id.ToString(),
                            Nickname = user.Nickname,
                            Name = user.Name,
                            Surname = user.Surname,
                            Description = user.Description,
                            BirthDate = DateTimeHelpers.GetDateFormattedFromDateTime(user.BirthDate),
                            ProfilePhotoFileId = user.ProfilePhotoFileId.ToString(),
                            Mail = user.Mail,
                            LastAccess = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(user.LastAccess),
                            ResidencePlaceId = user.ResidencePlace?.Id.ToString(),
                            ResidencePlaceName = user.ResidencePlace?.Name
                        };

                        // return
                        return userView;
                    }
                    else
                    {
                        throw new Exception("No User Found");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion


        #region USER
        [HttpGet]
        public async Task<List<ViewModels.User>> GetUserList(string searchText, bool isAlsoNotActive, bool isRandom, int takeOffset = 0, int takeCount = 999)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;

                return await _dbContext
                        .Users
                        .Where(x => (isAlsoNotActive || x.StatusEnum == Models.UserStatusEnum.Activated) && (EF.Functions.Like(x.Name + " " + x.Surname, "%" + searchText + "%") || EF.Functions.Like(x.Nickname, "%" + searchText + "%")))
                        .OrderByDescending(x => x.StatusEnum == Models.UserStatusEnum.Activated)
                        .ThenByDescending(x => isRandom ? Guid.NewGuid().ToString() : x.LastAccess.ToString())
                        /*
                        .ThenBy(x => searchText != null ? x.Name : Guid.NewGuid().ToString())
                        .ThenBy(x => x.Surname)
                        .Take(10)
                        */
                        .Skip(takeOffset)
                        .Take(takeCount)
                        .Include(x => x.ResidencePlace)
                        .Include(x => x.ProfilePhotoFile)
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
                            //Mail = x.Mail,
                            LastAccess = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.LastAccess),
                            ResidencePlaceId = x.ResidencePlace != null ? x.ResidencePlace.Id.ToString() : null,
                            ResidencePlaceName = x.ResidencePlace.Name,
                            ResidencePlaceDescription = x.ResidencePlace.Description,
                            ResidencePlaceLatitude = x.ResidencePlace.Latitude,
                            ResidencePlaceLongitude = x.ResidencePlace.Longitude,
                            StatusEnum = x.StatusEnum,
                            IsMine = isUserLogged && x.Id == userLoggedIdGuid,
                            IsFriend = isUserLogged && _dbContext.Users_UsersFriends.Any(u => ((u.UserId == userLoggedIdGuid && u.UserFriendId == x.Id) || (u.UserId == x.Id && u.UserFriendId == userLoggedIdGuid)) && u.StatusEnum == Models.UserFriendStatusEnum.Accepted)
                        }).ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<ViewModels.User> GetUser(string userId)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;
                var userIdGuid = Guid.Parse(userId);

                return await _dbContext
                        .Users
                        .Where(x => x.Id == userIdGuid)
                        .Include(x => x.ResidencePlace)
                        .Select(x => new ViewModels.User()
                        {
                            Id = x.Id.ToString(),
                            Nickname = x.Nickname,
                            Name = x.Name,
                            Surname = x.Surname,
                            Description = x.Description,
                            BirthDate = DateTimeHelpers.GetDateFormattedFromDateTime(x.BirthDate),
                            ProfilePhotoFileId = x.ProfilePhotoFileId.ToString(),
                            //Mail = x.Mail,
                            LastAccess = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.LastAccess),
                            ResidencePlaceId = x.ResidencePlace != null ? x.ResidencePlace.Id.ToString() : null,
                            ResidencePlaceName = x.ResidencePlace != null ? x.ResidencePlace.Name : null,
                            StatusEnum = x.StatusEnum,
                            IsMine = isUserLogged && x.Id == userLoggedIdGuid
                        }).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<FileStreamResult> GetUserPicture(string pictureId, bool isPreview)
        {
            try
            {
                var pictureIdGuid = Guid.Parse(pictureId);

                var picture = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == pictureIdGuid);
                if (picture == null)
                    throw new Exception("Picture not present");

                string path;
                if (isPreview && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, "Previews", picture.FileName)))
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, "Previews", picture.FileName);
                else if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, picture.FileName)))
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, picture.FileName);
                else
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, "Temp", picture.FileName);
                var image = System.IO.File.OpenRead(path);
                return File(image, picture.FileType);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<List<ViewModels.User>> GetUserToDoListByExperience(string experienceId, string searchText, int takeOffset = 0, int takeCount = 999)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;
                var experienceIdGuid = Guid.Parse(experienceId);

                return await _dbContext.Users_ExperiencesToDo
                       .Where(x => x.ExperienceId == experienceIdGuid && (EF.Functions.Like(x.User.Name + " " + x.User.Surname, "%" + searchText + "%") || EF.Functions.Like(x.User.Nickname, "%" + searchText + "%")))
                       .Include(x => x.User)
                       .ThenInclude(x => x.ResidencePlace)
                       /*
                       .OrderBy(x => searchText != null ? x.User.Name : Guid.NewGuid().ToString())
                       .ThenBy(x => x.User.Surname)
                       */
                       .OrderBy(x => x.User.Name)
                       .ThenBy(x => x.User.Surname)
                       .Skip(takeOffset)
                       .Take(takeCount)
                       .Select(x => new ViewModels.User()
                       {
                           Id = x.User.Id.ToString(),
                           Nickname = x.User.Nickname,
                           Name = x.User.Name,
                           Surname = x.User.Surname,
                           Description = x.User.Description,
                           BirthDate = DateTimeHelpers.GetDateFormattedFromDateTime(x.User.BirthDate),
                           ProfilePhotoFileId = x.User.ProfilePhotoFileId.ToString(),
                           //Mail = x.Mail,
                           LastAccess = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.User.LastAccess),
                           ResidencePlaceId = x.User.ResidencePlace != null ? x.User.ResidencePlace.Id.ToString() : null,
                           ResidencePlaceName = x.User.ResidencePlace != null ? x.User.ResidencePlace.Name : null,
                           StatusEnum = x.User.StatusEnum,
                           IsMine = isUserLogged && x.UserId == userLoggedIdGuid
                       }).Distinct().ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<List<ViewModels.User>> GetUserFriendListByUser(string userId, string searchText, bool isAlsoNotActive, bool isRandom, int takeOffset = 0, int takeCount = 999)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;
                var userIdGuid = Guid.Parse(userId);

                return await _dbContext.Users_UsersFriends
                       .Where(x =>
                            (isAlsoNotActive || (x.User.StatusEnum == Models.UserStatusEnum.Activated && x.UserFriend.StatusEnum == Models.UserStatusEnum.Activated))
                            && ((x.UserId == userIdGuid && (EF.Functions.Like(x.UserFriend.Name + " " + x.UserFriend.Surname, "%" + searchText + "%") || EF.Functions.Like(x.UserFriend.Nickname, "%" + searchText + "%")))
                            || (x.UserFriendId == userIdGuid && (EF.Functions.Like(x.User.Name + " " + x.User.Surname, "%" + searchText + "%") || EF.Functions.Like(x.User.Nickname, "%" + searchText + "%"))))
                       )
                       .Include(x => x.User)
                       .ThenInclude(x => x.ResidencePlace)
                       //.OrderBy(x => searchText != null ? x.User.Name : Guid.NewGuid().ToString())
                       //.OrderByDescending(x => x.StatusEnum == Models.UserFriendStatusEnum.Pending)
                       //.OrderBy(x => isRandom ? Guid.NewGuid().ToString() : searchText != null ? x.User.Name : ((int)x.StatusEnum).ToString())
                       //.ThenBy(x => x.User.Name)
                       //.ThenBy(x => x.User.Surname)
                       .OrderByDescending(x => x.User.StatusEnum == Models.UserStatusEnum.Activated)
                       .ThenByDescending(x => x.UserFriend.StatusEnum == Models.UserStatusEnum.Activated)
                       .ThenByDescending(x => isRandom ? Guid.NewGuid().ToString() : x.RequestedDateTime.ToString())
                       .Skip(takeOffset)
                       .Take(takeCount)
                       .Select(x => new ViewModels.User()
                       {
                           Id = x.UserId == userIdGuid ? x.UserFriend.Id.ToString() : x.User.Id.ToString(),
                           Nickname = x.UserId == userIdGuid ? x.UserFriend.Nickname : x.User.Nickname,
                           Name = x.UserId == userIdGuid ? x.UserFriend.Name : x.User.Name,
                           Surname = x.UserId == userIdGuid ? x.UserFriend.Surname : x.User.Surname,
                           Description = x.UserId == userIdGuid ? x.UserFriend.Description : x.User.Description,
                           BirthDate = x.UserId == userIdGuid ? DateTimeHelpers.GetDateFormattedFromDateTime(x.UserFriend.BirthDate) : DateTimeHelpers.GetDateFormattedFromDateTime(x.User.BirthDate),
                           ProfilePhotoFileId = x.UserId == userIdGuid ? x.UserFriend.ProfilePhotoFile.Id.ToString() : x.User.ProfilePhotoFile.Id.ToString(),
                           ProfilePhotoFileName = x.UserId == userIdGuid ? x.UserFriend.ProfilePhotoFile.FileName : x.User.ProfilePhotoFile.FileName,
                           ProfilePhotoFileType = x.UserId == userIdGuid ? x.UserFriend.ProfilePhotoFile.FileType : x.User.ProfilePhotoFile.FileType,
                           ResidencePlaceId = x.UserId == userIdGuid ? x.UserFriend.ResidencePlace.Id.ToString() : x.User.ResidencePlace.Id.ToString(),
                           ResidencePlaceName = x.UserId == userIdGuid ? x.UserFriend.ResidencePlace.Name : x.User.ResidencePlace.Name,
                           ResidencePlaceDescription = x.UserId == userIdGuid ? x.UserFriend.ResidencePlace.Description : x.User.ResidencePlace.Description,
                           ResidencePlaceLatitude = x.UserId == userIdGuid ? x.UserFriend.ResidencePlace.Latitude : x.User.ResidencePlace.Latitude,
                           ResidencePlaceLongitude = x.UserId == userIdGuid ? x.UserFriend.ResidencePlace.Longitude : x.User.ResidencePlace.Longitude,
                           StatusEnum = x.UserId == userIdGuid ? x.UserFriend.StatusEnum : x.User.StatusEnum,

                           IsFriend = x.StatusEnum == Models.UserFriendStatusEnum.Accepted,
                           FriendStatusEnum = x.StatusEnum,
                           FriendRequesterUserId = x.User.Id.ToString(),
                           FriendRecieverUserId = x.UserFriend.Id.ToString(),
                           FriendRequestedDateTime = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.RequestedDateTime),
                           FriendAcceptedDateTime = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.AcceptedDateTime),
                           FriendIsConfirmable = isUserLogged && x.UserFriend.Id == userLoggedIdGuid
                       }).ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        [SessionAuthorize]
        public async Task<ViewModels.User> GetCurrentUserFriend(string userId)
        {
            try
            {
                var userLoggedIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var userIdGuid = Guid.Parse(userId);

                var currentUserFriend = await _dbContext.Users_UsersFriends
                    .Where(x => (x.UserId == userIdGuid && x.UserFriendId == userLoggedIdGuid) || (x.UserId == userLoggedIdGuid && x.UserFriendId == userIdGuid))
                    .Select(x => new ViewModels.User()
                    {
                        Id = x.UserId == userIdGuid ? x.UserFriend.Id.ToString() : x.User.Id.ToString(),
                        Nickname = x.UserId == userIdGuid ? x.UserFriend.Nickname : x.User.Nickname,
                        Name = x.UserId == userIdGuid ? x.UserFriend.Name : x.User.Name,
                        Surname = x.UserId == userIdGuid ? x.UserFriend.Surname : x.User.Surname,
                        Description = x.UserId == userIdGuid ? x.UserFriend.Description : x.User.Description,
                        BirthDate = x.UserId == userIdGuid ? DateTimeHelpers.GetDateFormattedFromDateTime(x.UserFriend.BirthDate) : DateTimeHelpers.GetDateFormattedFromDateTime(x.User.BirthDate),
                        ProfilePhotoFileId = x.UserId == userIdGuid ? x.UserFriend.ProfilePhotoFile.Id.ToString() : x.User.ProfilePhotoFile.Id.ToString(),
                        //ProfilePhotoFileName = x.UserId == userIdGuid ? x.UserFriend.ProfilePhotoFile.FileName : x.User.ProfilePhotoFile.FileName,
                        //ProfilePhotoFileType = x.UserId == userIdGuid ? x.UserFriend.ProfilePhotoFile.FileType : x.User.ProfilePhotoFile.FileType,
                        ResidencePlaceId = x.UserId == userIdGuid ? x.UserFriend.ResidencePlace.Id.ToString() : x.User.ResidencePlace.Id.ToString(),
                        ResidencePlaceName = x.UserId == userIdGuid ? x.UserFriend.ResidencePlace.Name : x.User.ResidencePlace.Name,
                        StatusEnum = x.UserId == userIdGuid ? x.UserFriend.StatusEnum : x.User.StatusEnum,

                        FriendStatusEnum = x.StatusEnum,
                        FriendRequesterUserId = x.User.Id.ToString(),
                        FriendRecieverUserId = x.UserFriend.Id.ToString(),
                        FriendRequestedDateTime = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.RequestedDateTime),
                        FriendAcceptedDateTime = DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.AcceptedDateTime)
                    })
                    .FirstOrDefaultAsync();

                if (currentUserFriend == null) throw new MyException("User not friend");

                return currentUserFriend;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public async Task<bool> InsertUser([FromForm] ViewModels.User_InsertData user)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                var uploadedFile = user.ProfilePhotoFile;
                string fileName = null;
                Guid userIdGuid;
                Guid profilePhotoFileIdGuid = Guid.Empty;
                Guid residencePlaceIdGuid = Guid.Empty;

                try
                {
                    // check duplicate
                    user.Nickname = user.Nickname.Trim();
                    user.Mail = user.Mail.Trim();
                    var userCheckDuplicate = await _dbContext.Users.Where(x => x.Mail == user.Mail).FirstOrDefaultAsync();
                    if (userCheckDuplicate != null)
                    {
                        throw new MyException("Mail already registered");
                    }
                    userCheckDuplicate = await _dbContext.Users.Where(x => x.Nickname == user.Nickname).FirstOrDefaultAsync();
                    if (userCheckDuplicate != null)
                    {
                        throw new MyException("Nickname already registered");
                    }

                    userIdGuid = Guid.NewGuid();


                    // check exists folder Media, if not create
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures)))
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures));

                    // check, save the file
                    if (uploadedFile != null && uploadedFile.Length > 0)
                    {
                        profilePhotoFileIdGuid = Guid.NewGuid();
                        fileName = DateTimeOffset.Now.ToUnixTimeMilliseconds() + uploadedFile.FileName;

                        // insert the file into db
                        var fileNew = new DataModels.Files
                        {
                            Id = profilePhotoFileIdGuid,
                            FileName = fileName,
                            FileType = uploadedFile.ContentType,
                            FileCreated = DateTimeOffset.Now
                        };
                        await _dbContext.Files.AddAsync(fileNew);
                    }


                    // check and, in case, insert the place into db
                    if (user.ResidencePlaceName != null && user.ResidencePlaceName != "")
                    {
                        var place = await _dbContext.Places.FirstOrDefaultAsync(x => x.Name == user.ResidencePlaceName && x.Latitude == user.ResidencePlaceLatitude && x.Longitude == user.ResidencePlaceLongitude);
                        if (place != null)
                        {
                            residencePlaceIdGuid = place.Id;
                        }
                        else
                        {
                            residencePlaceIdGuid = Guid.NewGuid();

                            await _dbContext.Places.AddAsync(new DataModels.Places
                            {
                                Id = residencePlaceIdGuid,
                                Name = user.ResidencePlaceName,
                                Description = user.ResidencePlaceDescription,
                                Latitude = user.ResidencePlaceLatitude,
                                Longitude = user.ResidencePlaceLongitude,
                            });
                        }
                    }

                    // insert the user into db
                    var userNew = new DataModels.Users
                    {
                        Id = userIdGuid,
                        Nickname = user.Nickname,
                        Name = user.Name,
                        Surname = user.Surname,
                        Description = user.Description,
                        BirthDate = DateTimeHelpers.GetDateTimeFromDateFormatted(user.BirthDate),
                        Mail = user.Mail,
                        Password = SecurePasswordHasher.Hash(user.Password),
                        StatusEnum = Models.UserStatusEnum.Default,
                        RegistrationDateTime = DateTimeOffset.Now
                    };
                    if (profilePhotoFileIdGuid != Guid.Empty)
                    {
                        userNew.ProfilePhotoFileId = profilePhotoFileIdGuid;
                    }
                    if (residencePlaceIdGuid != Guid.Empty)
                    {
                        userNew.ResidencePlaceId = residencePlaceIdGuid;
                    }

                    await _dbContext.Users.AddAsync(userNew);


                    await _dbContext.SaveChangesAsync();


                    // save file and delete old one
                    if (fileName != null)
                    {
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures);
                        await MediaEditor.SaveMedia(uploadedFile, folderPath, fileName);
                    }


                    // try to send the mail
                    // TODO: non mi piace assai
                    string activationCode = null;
                    do
                    {
                        activationCode = SecurePasswordHasher.Hash(user.Mail);
                    } while (activationCode.Contains("+"));
                    var activationLink = Request.Scheme + "://" + Request.Host + "/Status/ActivateUser?userId=" + userIdGuid + "&activationCode=" + activationCode;


                    var smtpServer = new SmtpClient(SMTPMail_Server, SMTPMail_Port);
                    smtpServer.Credentials = new System.Net.NetworkCredential(SMTPMail_Username, SMTPMail_Passwrod);
                    smtpServer.EnableSsl = SMTPMail_IsSSL;
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("info@worldisawesome.fun");
                    mail.To.Add(user.Mail);
                    mail.Subject = "World is Awesome - Registration";
                    mail.IsBodyHtml = true;
                    mail.Body = "The registration is completed! Now click this link for activate your account and start to use www.worldisawesome.fun";
                    mail.Body += "<br/><br/>";
                    mail.Body += "<a href='" + activationLink + "'>" + activationLink + "</a>";
                    mail.Body += "<br/><br/>";
                    mail.Body += "Thank you. World is Awesome .fun Team <br/>";

                    smtpServer.Send(mail);


                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();

                    // elimino anche i file in caso di errore invio mail
                    if (fileName != null)
                    {
                        var oldPicturePath = Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, fileName);
                        var oldPicturePreviewPath = Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, "Previews", fileName);
                        if (System.IO.File.Exists(oldPicturePath))
                        {
                            System.IO.File.Delete(oldPicturePath);
                        }
                        if (System.IO.File.Exists(oldPicturePreviewPath))
                        {
                            System.IO.File.Delete(oldPicturePreviewPath);
                        }
                    }

                    throw e;
                }
            }
        }
        [HttpPost]
        [SessionAuthorize]
        public async Task<bool> UpdateUser_PersonalInformations([FromForm] ViewModels.User_UpdatePersonalInformationsData user)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                    var uploadedFile = user.ProfilePhotoFile;
                    string fileName = null;
                    Guid oldFileId = Guid.Empty;
                    Guid profilePhotoFileIdGuid = Guid.Empty;
                    Guid residencePlaceIdGuid = Guid.Empty;


                    // check user
                    if (Guid.Parse(user.UserId) != userIdGuid)
                    {
                        throw new Exception("It s not allowed to edit the user sent");
                    }

                    // check exists folder Media, if not create
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures)))
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures));

                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, "Previews")))
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures, "Previews"));

                    // check, save the file
                    if (user.ProfilePhotoFileId != null && (uploadedFile == null || uploadedFile.Length == 0))
                    {
                        // check if the file is correct
                        var checkFileIdGuid = Guid.Parse(user.ProfilePhotoFileId);
                        var checkFile = await _dbContext.Files
                            .Include(x => x.Users)
                            .Where(x => x.Users.FirstOrDefault(y => y.Id == userIdGuid) != null && x.Id == checkFileIdGuid)
                            .FirstOrDefaultAsync();
                        if (checkFile == null)
                        {
                            throw new Exception("File is not correct or is not property of current user");
                        }

                        profilePhotoFileIdGuid = checkFileIdGuid;
                    }
                    else if (uploadedFile != null && uploadedFile.Length > 0)
                    {
                        profilePhotoFileIdGuid = Guid.NewGuid();
                        fileName = DateTimeOffset.Now.ToUnixTimeMilliseconds() + uploadedFile.FileName;

                        // insert the file into db
                        var fileNew = new DataModels.Files
                        {
                            Id = profilePhotoFileIdGuid,
                            FileName = fileName,
                            FileType = uploadedFile.ContentType,
                            FileCreated = DateTimeOffset.Now
                        };
                        await _dbContext.Files.AddAsync(fileNew);
                    }
                    else
                    {
                        // file not selected -> to delete
                        profilePhotoFileIdGuid = Guid.Empty;
                    }


                    // check and, in case, insert the place into db
                    if (user.ResidencePlaceId != null && (user.ResidencePlaceName == null || user.ResidencePlaceName == ""))
                    {
                        residencePlaceIdGuid = Guid.Parse(user.ResidencePlaceId);
                    }
                    else if (user.ResidencePlaceName != null && user.ResidencePlaceName != "")
                    {
                        var place = await _dbContext.Places.FirstOrDefaultAsync(x => x.Name == user.ResidencePlaceName && x.Latitude == user.ResidencePlaceLatitude && x.Longitude == user.ResidencePlaceLongitude);
                        if (place != null)
                        {
                            residencePlaceIdGuid = place.Id;
                        }
                        else
                        {
                            residencePlaceIdGuid = Guid.NewGuid();

                            await _dbContext.Places.AddAsync(new DataModels.Places
                            {
                                Id = residencePlaceIdGuid,
                                Name = user.ResidencePlaceName,
                                Description = user.ResidencePlaceDescription,
                                Latitude = user.ResidencePlaceLatitude,
                                Longitude = user.ResidencePlaceLongitude,
                            });
                        }
                    }
                    else
                    {
                        // file not selected -> to delete
                        residencePlaceIdGuid = Guid.Empty;
                    }

                    // check, insert the experience into db
                    if (userIdGuid != null)
                    {
                        var userUpdate = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userIdGuid);

                        // check if it has an old media, if there is delete it in the end
                        if (userUpdate.ProfilePhotoFileId != null && userUpdate.ProfilePhotoFileId != profilePhotoFileIdGuid)
                        {
                            oldFileId = Guid.Parse(userUpdate.ProfilePhotoFileId.ToString());
                        }

                        // userUpdate.Nickname = user.Nickname;
                        userUpdate.Name = user.Name;
                        userUpdate.Surname = user.Surname;
                        userUpdate.Description = user.Description;
                        userUpdate.BirthDate = DateTimeHelpers.GetDateTimeFromDateFormatted(user.BirthDate);

                        if (profilePhotoFileIdGuid != Guid.Empty)
                        {
                            userUpdate.ProfilePhotoFileId = profilePhotoFileIdGuid;
                        }
                        else
                        {
                            userUpdate.ProfilePhotoFileId = null;
                        }
                        if (residencePlaceIdGuid != Guid.Empty)
                        {
                            userUpdate.ResidencePlaceId = residencePlaceIdGuid;
                        }
                        else
                        {
                            userUpdate.ResidencePlaceId = null;
                        }

                        _dbContext.Users.Update(userUpdate);
                    }
                    else
                    {
                        // no user logged
                        throw new Exception("User is not logged");
                    }

                    await _dbContext.SaveChangesAsync();


                    // save file and delete old one
                    if (fileName != null)
                    {
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures);
                        await MediaEditor.SaveMedia(uploadedFile, folderPath, fileName);
                    }
                    if (oldFileId != Guid.Empty)
                    {
                        var oldPicture = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == oldFileId);
                        _dbContext.Files.Remove(oldPicture);
                        await _dbContext.SaveChangesAsync();

                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), Folder_UsersPictures);
                        MediaEditor.DeleteMedia(folderPath, oldPicture.FileName);
                    }


                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw e;
                }
            }
        }
        [HttpPost]
        [SessionAuthorize]
        public async Task<bool> UpdateUser_Password([FromForm] ViewModels.User_UpdatePasswordData user)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    var password = user.Password.Trim();
                    var passwordNew = user.PasswordNew.Trim();
                    DataModels.Users userUpdate;

                    // check user
                    if (userIdGuid != null)
                    {
                        // check, insert the experience into db
                        userUpdate = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userIdGuid);

                        if (SecurePasswordHasher.Verify(password, userUpdate.Password))
                        {
                            userUpdate.Password = SecurePasswordHasher.Hash(user.PasswordNew);

                            _dbContext.Users.Update(userUpdate);
                        }
                        else
                        {
                            throw new MyException("Password of User is wrong");
                        }
                    }
                    else
                    {
                        throw new MyException("User is not logged or is a wrong user");
                    }

                    await _dbContext.SaveChangesAsync();


                    var smtpServer = new SmtpClient(SMTPMail_Server, SMTPMail_Port);
                    smtpServer.Credentials = new System.Net.NetworkCredential(SMTPMail_Username, SMTPMail_Passwrod);
                    smtpServer.EnableSsl = SMTPMail_IsSSL;
                    var mailMessageFrom = "info@worldisawesome.fun";
                    var mailMessageSubject = "World is Awesome - Change password";
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(mailMessageFrom);
                    mail.To.Add(userUpdate.Mail);
                    mail.Subject = mailMessageSubject;
                    mail.IsBodyHtml = true;
                    mail.Body = "You changed your password! Write your new password in a place where you can't lose it anymore";
                    mail.Body += "<br/><br/>";
                    mail.Body += "If you have not done this operation, please contact us replying to this email";
                    mail.Body += "<br/><br/>";
                    mail.Body += "Thank you. World is Awesome .fun Team <br/>";
                    smtpServer.Send(mail);


                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw e;
                }
            }
        }
        [HttpPost]
        [SessionAuthorize]
        public async Task<bool> UpdateUser_Mail([FromForm] ViewModels.User_UpdateMailData user)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    var password = user.Password;
                    var mailNew = user.MailNew.Trim();
                    string mailOld;

                    // check user
                    if (userIdGuid != null)
                    {
                        // check, insert the experience into db
                        var userUpdate = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userIdGuid);

                        if (SecurePasswordHasher.Verify(password, userUpdate.Password))
                        {
                            var userCheckDuplicate = await _dbContext.Users.Where(x => x.Mail == mailNew).FirstOrDefaultAsync();
                            if (userCheckDuplicate != null)
                            {
                                throw new MyException("New mail already registered");
                            }

                            mailOld = userUpdate.Mail;
                            userUpdate.Mail = mailNew;
                            userUpdate.StatusEnum = Models.UserStatusEnum.Default;

                            _dbContext.Users.Update(userUpdate);
                        }
                        else
                        {
                            throw new MyException("Password of User is wrong");
                        }
                    }
                    else
                    {
                        throw new MyException("User is not logged or is a wrong user");
                    }

                    await _dbContext.SaveChangesAsync();


                    // send mails
                    var smtpServer = new SmtpClient(SMTPMail_Server, SMTPMail_Port);
                    smtpServer.Credentials = new System.Net.NetworkCredential(SMTPMail_Username, SMTPMail_Passwrod);
                    smtpServer.EnableSsl = SMTPMail_IsSSL;

                    var mailMessageFrom = "info@worldisawesome.fun";
                    var mailMessageSubject = "World is Awesome - Change mail";

                    // try to send the old mail
                    MailMessage mailMessageOld = new MailMessage();
                    mailMessageOld.From = new MailAddress(mailMessageFrom);
                    mailMessageOld.To.Add(mailOld);
                    mailMessageOld.Subject = mailMessageSubject;
                    mailMessageOld.IsBodyHtml = true;
                    mailMessageOld.Body = "You changed your mail! A mail will be sent to the new mail address: " + mailNew;
                    mailMessageOld.Body += "<br/><br/>";
                    mailMessageOld.Body += "If you have not done this operation, please contact us replying to this email";
                    mailMessageOld.Body += "<br/><br/>";
                    mailMessageOld.Body += "Thank you. World is Awesome .fun Team <br/>";
                    smtpServer.Send(mailMessageOld);

                    // try to send the new mail
                    // TODO: non mi piace assai
                    string activationCode = null;
                    do
                    {
                        activationCode = SecurePasswordHasher.Hash(mailNew);
                    } while (activationCode.Contains("+"));
                    var activationLink = Request.Scheme + "://" + Request.Host + "/Status/ActivateUser?userId=" + userIdGuid + "&activationCode=" + activationCode;

                    MailMessage mailMessageNew = new MailMessage();
                    mailMessageNew.From = new MailAddress(mailMessageFrom);
                    mailMessageNew.To.Add(mailNew);
                    mailMessageNew.Subject = mailMessageSubject;
                    mailMessageNew.IsBodyHtml = true;
                    mailMessageNew.Body = "You changed your mail! Now click this link for activate your account and start to use www.worldisawesome.fun";
                    mailMessageNew.Body += "<br/><br/>";
                    mailMessageNew.Body += "<a href='" + activationLink + "'>" + activationLink + "</a>";
                    mailMessageNew.Body += "<br/><br/>";
                    mailMessageNew.Body += "Thank you. World is Awesome .fun Team <br/>";
                    smtpServer.Send(mailMessageNew);



                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw e;
                }
            }
        }
        [HttpGet]
        [SessionAuthorize]
        public async Task<bool> RequestUser_Contact(string userId, string message)
        {
            try
            {
                var userIdGuid = Guid.Parse(userId);
                var userLoggedIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                var userLogged = await _dbContext
                        .Users
                        .Where(x => x.Id == userLoggedIdGuid && x.StatusEnum == Models.UserStatusEnum.Activated)
                        .Select(x => new ViewModels.User()
                        {
                            Id = x.Id.ToString(),
                            Nickname = x.Nickname,
                            Name = x.Name,
                            Surname = x.Surname,
                            Mail = x.Mail,
                        }).FirstOrDefaultAsync();
                if (userLogged == null)
                {
                    throw new MyException("User not logged or not active");
                }

                var user = await _dbContext
                        .Users
                        .Where(x => x.Id == userIdGuid && x.StatusEnum == Models.UserStatusEnum.Activated)
                        .Select(x => new ViewModels.User()
                        {
                            Id = x.Id.ToString(),
                            Nickname = x.Nickname,
                            Name = x.Name,
                            Surname = x.Surname,
                            Mail = x.Mail,
                            StatusEnum = x.StatusEnum,
                        }).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new MyException("User doesn t exist or is not active");
                }

                // send mails
                var smtpServer = new SmtpClient(SMTPMail_Server, SMTPMail_Port);
                smtpServer.Credentials = new System.Net.NetworkCredential(SMTPMail_Username, SMTPMail_Passwrod);
                smtpServer.EnableSsl = SMTPMail_IsSSL;

                var mailMessageFrom = "info@worldisawesome.fun";
                var mailMessageSubject = "World is Awesome - Contact Request";
                var mailMessageTo = user.Mail;

                // link user
                var userLink = Request.Scheme + "://" + Request.Host + "/View/User?UserId=" + userLogged.Id;

                // try to send the old mail
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(mailMessageFrom);
                mail.To.Add(mailMessageTo);
                mail.Subject = mailMessageSubject;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.Body = "Hi <b>" + user.Nickname + "</b>!<br/>";
                mail.Body += "I'm <b>" + userLogged.Nickname + "</b> and I contact you from <b>World is Awesome</b>!";
                mail.Body += "<br/><br/><br/>";
                mail.Body += "<small>" + userLogged.Nickname + " message:</small><br/>";
                mail.Body += "<em>" + message + "</em>";
                mail.Body += "<br/><br/>";
                mail.Body += "This is my mail! Answer me!<br/>";
                mail.Body += "<a href='mailto:" + userLogged.Mail + "'>" + userLogged.Mail + "</a>";
                mail.Body += "<br/><br/>";
                mail.Body += "You can see more of me in my profile, click this link!<br/>";
                mail.Body += "<a href='" + userLink + "'>" + userLink + "</a>";
                mail.Body += "<br/><br/><br/>";
                mail.Body += "Thank you. World is Awesome .fun Team <br/>";
                smtpServer.Send(mail);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        [SessionAuthorize]
        public async Task<bool> AddUserExperienceToDo(string experienceId)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    var experienceIdGuid = Guid.Parse(experienceId);

                    // check if is already added
                    var checkExperience = await _dbContext.Users_ExperiencesToDo
                        .Where(x => x.UserId == userIdGuid && x.ExperienceId == experienceIdGuid)
                        .FirstOrDefaultAsync();

                    if (checkExperience != null)
                    {
                        throw new MyException("Experience already in user ToDo List");
                    }
                    else
                    {
                        var userExperienceToDoNew = new DataModels.Users_ExperiencesToDo
                        {
                            UserId = userIdGuid,
                            ExperienceId = experienceIdGuid
                        };
                        await _dbContext.Users_ExperiencesToDo.AddAsync(userExperienceToDoNew);

                        await _dbContext.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();

                    throw e;
                }
            }
        }
        [HttpGet]
        [SessionAuthorize]
        public async Task<bool> RemoveUserExperienceToDo(string experienceId)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    var experienceIdGuid = Guid.Parse(experienceId);

                    var userExperienceToDo = await _dbContext.Users_ExperiencesToDo.FirstOrDefaultAsync(x => x.UserId == userIdGuid && x.ExperienceId == experienceIdGuid);
                    _dbContext.Users_ExperiencesToDo.Remove(userExperienceToDo);

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();

                    throw e;
                }
            }
        }

        [HttpGet]
        [SessionAuthorize]
        public async Task<bool> AddUserFriend(string userFriendId)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    var userFriendIdGuid = Guid.Parse(userFriendId);

                    // check if is already added
                    var checkFriend = await _dbContext.Users_UsersFriends
                        .Where(x => (x.UserId == userIdGuid && x.UserFriendId == userFriendIdGuid) || (x.UserId == userFriendIdGuid && x.UserFriendId == userIdGuid))
                        .FirstOrDefaultAsync();

                    if (checkFriend != null)
                    {
                        throw new MyException("User already in friend List");
                    }
                    else
                    {
                        var userFriendNew = new DataModels.Users_UsersFriends
                        {
                            UserId = userIdGuid,
                            UserFriendId = userFriendIdGuid,
                            RequestedDateTime = DateTimeOffset.Now
                        };
                        await _dbContext.Users_UsersFriends.AddAsync(userFriendNew);

                        await _dbContext.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();

                    throw e;
                }
            }
        }
        [HttpGet]
        [SessionAuthorize]
        public async Task<bool> RemoveUserFriend(string userFriendId)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    var userFriendIdGuid = Guid.Parse(userFriendId);

                    var userFriend = await _dbContext.Users_UsersFriends.FirstOrDefaultAsync(x => (x.UserId == userIdGuid && x.UserFriendId == userFriendIdGuid) || (x.UserId == userFriendIdGuid && x.UserFriendId == userIdGuid));
                    _dbContext.Users_UsersFriends.Remove(userFriend);

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();

                    throw e;
                }
            }
        }
        [HttpGet]
        [SessionAuthorize]
        public async Task<bool> ConfirmUserFriend(string userFriendId)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    var userFriendIdGuid = Guid.Parse(userFriendId);

                    // check if is exists
                    var checkFriend = await _dbContext.Users_UsersFriends
                        .Where(x => (x.UserId == userFriendIdGuid && x.UserFriendId == userIdGuid) && x.StatusEnum == Models.UserFriendStatusEnum.Pending)
                        .FirstOrDefaultAsync();

                    if (checkFriend == null)
                    {
                        throw new MyException("User friend doesn t exists or is not in pending request");
                    }
                    else
                    {
                        checkFriend.StatusEnum = Models.UserFriendStatusEnum.Accepted;
                        checkFriend.AcceptedDateTime = DateTimeOffset.Now;
                        _dbContext.Users_UsersFriends.Update(checkFriend);

                        await _dbContext.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();

                    throw e;
                }
            }
        }
        #endregion


        #region place
        [HttpGet]
        public async Task<List<ViewModels.Place>> GetPlaceList(Models.MorningNightEnum morningNightEnum, bool isToDo, string searchText, string userId, bool isRandom, int takeOffset = 0, int takeCount = 999)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;

                var userIdGuid = userId == null ? Guid.Empty : Guid.Parse(userId);

                return await _dbContext
                        .View_PlaceExperiences
                        .Where(x =>
                            (isToDo || x.UserId == userIdGuid || userIdGuid == Guid.Empty) &&
                            (!isToDo || _dbContext.Users_ExperiencesToDo.Any(ue => ue.UserId == userIdGuid && ue.ExperienceId == x.ExperienceId)) &&
                            (morningNightEnum == Models.MorningNightEnum.Default || x.ExperienceMorningNightEnum == morningNightEnum) && (EF.Functions.Like(x.PlaceName, "%" + searchText + "%")) &&
                            ((isUserLogged && x.UserId == userLoggedIdGuid) || x.ExperienceStatusEnum == Models.ExperienceStatusEnum.Completed) &&
                            (x.ExperiencePrivacyLevel == Models.PrivacyLevelEnum.Public || (isUserLogged && x.UserId == userLoggedIdGuid) || (isUserLogged && x.ExperiencePrivacyLevel == Models.PrivacyLevelEnum.Confidential && _dbContext.Users_UsersFriends.Any(uf => ((uf.UserId == userLoggedIdGuid && uf.UserFriendId == x.UserId) || (uf.UserId == x.UserId && uf.UserFriendId == userLoggedIdGuid)) && uf.StatusEnum == Models.UserFriendStatusEnum.Accepted)))
                        )
                        .GroupBy(x => new { x.PlaceId, x.PlaceName, x.PlaceDescription, x.PlaceLatitude, x.PlaceLongitude, x.PlaceProfilePictureFileId/*, x.UserId*/ })
                        //.OrderBy(x => searchText != null ? x.Key.PlaceName : Guid.NewGuid().ToString())
                        //.Take(userIdGuid == Guid.Empty ? 10 : 999)
                        .OrderBy(x => isRandom ? Guid.NewGuid().ToString() : x.Key.PlaceName)
                        .Skip(takeOffset)
                        .Take(takeCount)
                        .Select(x => new ViewModels.Place()
                        {
                            Id = x.Key.PlaceId.ToString(),
                            Name = x.Key.PlaceName,
                            Description = x.Key.PlaceDescription,
                            Latitude = x.Key.PlaceLatitude,
                            Longitude = x.Key.PlaceLongitude,
                            PictureId = x.Key.PlaceProfilePictureFileId.ToString(),
                            IsMine = (isUserLogged && _dbContext.Experiences.Any(e => e.UserId == userLoggedIdGuid && e.PlaceId == x.Key.PlaceId))
                            // Experience_Count = x.Count()
                        }).ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<FileStreamResult> GetPlacePicture(string pictureId, bool isPreview)
        {
            try
            {
                var pictureIdGuid = Guid.Parse(pictureId);

                var picture = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == pictureIdGuid);
                if (picture == null)
                    throw new Exception("Picture not present");

                string path;
                if (isPreview && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_PlacesPictures, "Previews", picture.FileName)))
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_PlacesPictures, "Previews", picture.FileName);
                else if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_PlacesPictures, picture.FileName)))
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_PlacesPictures, picture.FileName);
                else
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_PlacesPictures, "Temp", picture.FileName);
                var image = System.IO.File.OpenRead(path);
                return File(image, picture.FileType);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<FileStreamResult> GetDefaultPlacePicture(string placeId, bool isPreview)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;

                var placeIdGuid = Guid.Parse(placeId);

                var place = await _dbContext.Places.FirstOrDefaultAsync(x => x.Id == placeIdGuid);

                if (place.ProfilePictureFileId != null)
                {
                    return await GetPlacePicture(place.ProfilePictureFileId.ToString(), isPreview);
                }
                else
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
                        return await GetExperienceMedia(file.FileId.ToString(), isPreview);
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


        #region experiences
        [HttpGet]
        public async Task<List<ViewModels.Experience>> GetExperienceList(string placeId, Models.MorningNightEnum morningNightEnum, string searchText, bool isToDo, string userId, bool isAlsoDraft, bool isRandom, int takeOffset = 0, int takeCount = 999)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;

                var userIdGuid = userId == null ? Guid.Empty : Guid.Parse(userId);
                var placeIdGuid = placeId == null ? Guid.Empty : Guid.Parse(placeId);

                return await _dbContext.Experiences
                    .Include(x => x.Place)
                    .Include(x => x.User)
                    // .Include(x => x.UsersToDo)
                    .Include(x => x.MediaFile)
                    .Where(x =>
                        ((isAlsoDraft && isUserLogged && x.UserId == userLoggedIdGuid) || x.StatusEnum == Models.ExperienceStatusEnum.Completed) &&
                        (placeIdGuid == Guid.Empty || x.PlaceId == placeIdGuid) &&
                        (morningNightEnum == Models.MorningNightEnum.Default || x.MorningNightEnum == morningNightEnum) &&
                        (isToDo || x.UserId == userIdGuid || userIdGuid == Guid.Empty) &&
                        (!isToDo || _dbContext.Users_ExperiencesToDo.Any(ue => ue.UserId == userIdGuid && ue.ExperienceId == x.Id)) &&
                        (EF.Functions.Like(x.Name, "%" + searchText + "%") || EF.Functions.Like(x.Description, "%" + searchText + "%")) &&
                        (x.PrivacyLevel == Models.PrivacyLevelEnum.Public || (isUserLogged && x.UserId == userLoggedIdGuid) || (isUserLogged && x.PrivacyLevel == Models.PrivacyLevelEnum.Confidential && _dbContext.Users_UsersFriends.Any(uf => ((uf.UserId == userLoggedIdGuid && uf.UserFriendId == x.UserId) || (uf.UserId == x.UserId && uf.UserFriendId == userLoggedIdGuid)) && uf.StatusEnum == Models.UserFriendStatusEnum.Accepted)))
                    )
                    .OrderByDescending(x => x.StatusEnum == Models.ExperienceStatusEnum.Draft)
                    .ThenByDescending(x => isRandom ? Guid.NewGuid().ToString() : x.Date.ToString())
                    .ThenByDescending(x => x.Time)
                    //.Take(userIdGuid == Guid.Empty ? 10 : 999)
                    .Skip(takeOffset)
                    .Take(takeCount)
                    .Select(x => new ViewModels.Experience()
                    {
                        Id = x.Id.ToString(),
                        Name = x.Name,
                        Description = x.Description,
                        Date = DateTimeHelpers.GetDateFormattedFromDateTime(x.Date),
                        Time = DateTimeHelpers.GetTimeFormattedFromTimeSpan(x.Time.Value),
                        MorningNightEnum = x.MorningNightEnum,
                        PrivacyLevel = x.PrivacyLevel,
                        DateTimeCreated = (double)DateTimeHelpers.GetMillisecondsFromDateTimeOffset(x.DateTimeCreated),
                        StatusEnum = x.StatusEnum,
                        IsMine = isUserLogged && x.UserId == userLoggedIdGuid,
                        FileId = x.MediaFile.Id.ToString(),
                        FileName = x.MediaFile.FileName,
                        FileType = x.MediaFile.FileType,
                        // File = x.MediaFile,
                        PlaceId = x.Place.Id.ToString(),
                        PlaceName = x.Place.Name,
                        PlaceDescription = x.Place.Description,
                        PlaceLatitude = x.Place.Latitude,
                        PlaceLongitude = x.Place.Longitude,
                        UserId = x.User.Id.ToString(),
                        UserNickname = x.User.Nickname,
                        UserName = x.User.Name,
                        UserSurname = x.User.Surname,
                        UsersToDoList = x.UsersToDo.Select(x => new ViewModels.User()
                        {
                            Id = x.User.Id.ToString(),
                            Nickname = x.User.Nickname,
                            Name = x.User.Name,
                            Surname = x.User.Surname
                        }).ToList()
                    })
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<ViewModels.Experience> GetExperience(string experienceId)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;

                var experienceIdGuid = Guid.Parse(experienceId);

                return await _dbContext.Experiences
                    .Where(x =>
                        x.Id == experienceIdGuid &&
                        ((isUserLogged && x.UserId == userLoggedIdGuid) || x.StatusEnum == Models.ExperienceStatusEnum.Completed) &&
                        ((isUserLogged && x.UserId == userLoggedIdGuid) || (isUserLogged && x.PrivacyLevel == Models.PrivacyLevelEnum.Confidential && _dbContext.Users_UsersFriends.Any(uf => ((uf.UserId == userLoggedIdGuid && uf.UserFriendId == x.UserId) || (uf.UserId == x.UserId && uf.UserFriendId == userLoggedIdGuid)) && uf.StatusEnum == Models.UserFriendStatusEnum.Accepted)) || x.PrivacyLevel == Models.PrivacyLevelEnum.Public))
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
                        StatusEnum = x.StatusEnum,
                        IsMine = isUserLogged && x.UserId == userLoggedIdGuid,
                        FileId = x.MediaFile.Id.ToString(),
                        FileName = x.MediaFile.FileName,
                        FileType = x.MediaFile.FileType,
                        // File = x.MediaFile,
                        PlaceId = x.Place.Id.ToString(),
                        PlaceName = x.Place.Name,
                        PlaceDescription = x.Place.Description,
                        PlaceLatitude = x.Place.Latitude,
                        PlaceLongitude = x.Place.Longitude,
                        UserId = x.User.Id.ToString(),
                        UserNickname = x.User.Nickname,
                        UserName = x.User.Name,
                        UserSurname = x.User.Surname,
                        UsersToDoList = x.UsersToDo.Select(x => new ViewModels.User()
                        {
                            Id = x.User.Id.ToString(),
                            Nickname = x.User.Nickname,
                            Name = x.User.Name,
                            Surname = x.User.Surname
                        }).ToList()
                    }).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<FileStreamResult> GetExperienceMedia(string mediaId, bool isPreview)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;

                var mediaIdGuid = Guid.Parse(mediaId);

                var media = await _dbContext.View_ExperienceFiles
                    .FirstOrDefaultAsync(x => x.FileId == mediaIdGuid &&
                        ((isUserLogged && x.UserId == userLoggedIdGuid) || x.ExperienceStatusEnum == Models.ExperienceStatusEnum.Completed) &&
                        (x.ExperiencePrivacyLevel == Models.PrivacyLevelEnum.Public || (isUserLogged && x.UserId == userLoggedIdGuid) || (isUserLogged && x.ExperiencePrivacyLevel == Models.PrivacyLevelEnum.Confidential && _dbContext.Users_UsersFriends.Any(uf => ((uf.UserId == userLoggedIdGuid && uf.UserFriendId == x.UserId) || (uf.UserId == x.UserId && uf.UserFriendId == userLoggedIdGuid)) && uf.StatusEnum == Models.UserFriendStatusEnum.Accepted)))
                        );
                if (media == null)
                    throw new MyException("Media not present");

                string path;
                if (isPreview && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, "Previews", media.FileName)))
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, "Previews", media.FileName);
                else if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, media.FileName)))
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, media.FileName);
                else
                    path = Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, "Temp", media.FileName);
                var image = System.IO.File.OpenRead(path);
                return File(image, media.FileType);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<ViewModels.File> GetDefaultExperienceMediaByPlace(string placeId, Models.MorningNightEnum morningNightEnum, bool isToDo, string userId)
        {
            try
            {
                var isUserLogged = (HttpContext.User.Claims?.Any() == true);
                var userLoggedIdGuid = isUserLogged ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) : Guid.Empty;

                var userIdGuid = userId == null ? Guid.Empty : Guid.Parse(userId);
                var placeIdGuid = Guid.Parse(placeId);

                return await _dbContext.View_ExperienceFiles
                    .Where(x => x.PlaceId == placeIdGuid &&
                        (morningNightEnum == Models.MorningNightEnum.Default || x.ExperienceMorningNightEnum == morningNightEnum) &&
                        (isToDo || x.UserId == userIdGuid || userIdGuid == Guid.Empty) &&
                        (!isToDo || x.UserToDo == userIdGuid) &&
                        ((isUserLogged && x.UserId == userLoggedIdGuid) || x.ExperienceStatusEnum == Models.ExperienceStatusEnum.Completed) &&
                        (x.ExperiencePrivacyLevel == Models.PrivacyLevelEnum.Public || (isUserLogged && x.UserId == userLoggedIdGuid) || (isUserLogged && x.ExperiencePrivacyLevel == Models.PrivacyLevelEnum.Confidential && _dbContext.Users_UsersFriends.Any(uf => ((uf.UserId == userLoggedIdGuid && uf.UserFriendId == x.UserId) || (uf.UserId == x.UserId && uf.UserFriendId == userLoggedIdGuid)) && uf.StatusEnum == Models.UserFriendStatusEnum.Accepted))))
                    .OrderBy(x => Guid.NewGuid())
                    .Select(x => new ViewModels.File()
                    {
                        Id = x.FileId.ToString(),
                        FileName = x.FileName,
                        FileType = x.FileType,
                        IsMine = isUserLogged && x.UserId == userLoggedIdGuid
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpPost]
        [SessionAuthorize]
        public async Task<bool> UpsertExperience([FromForm] ViewModels.Experience experience)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

                    var uploadedFile = experience.File;
                    Guid experienceIdGuid;
                    Guid? fileIdGuid = null;
                    string fileName = null;
                    Guid? placeIdGuid = null;

                    Guid? oldFileId = null;


                    // check if has all field set
                    if (experience.StatusEnum == Models.ExperienceStatusEnum.Completed)
                    {
                        if (experience.Name == null || experience.Name == "") throw new MyException("Required field: Name not set");
                        if (!(experience.FileId != null || (uploadedFile != null && uploadedFile.Length > 0))) throw new MyException("Required field: File not set");
                        if (experience.Description == null || experience.Description == "") throw new MyException("Required field: Description not set");
                        if (experience.Date == null || experience.Date == "") throw new MyException("Required field: Date not set");
                        if (experience.MorningNightEnum == null || experience.MorningNightEnum == Models.MorningNightEnum.Default) throw new MyException("Required field: MorningNightEnum not set");
                        if (!(experience.PlaceId != null || (experience.PlaceName != null && experience.PlaceName != ""))) throw new MyException("Required field: Place not set");
                        if (experience.PrivacyLevel == null || experience.PrivacyLevel == Models.PrivacyLevelEnum.Default) throw new MyException("Required field: PrivacyLevel not set");
                    }

                    // check exists folder Media, if not create
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia)))
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia));
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, "Previews")))
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, "Previews"));

                    // check, save the file
                    if (experience.FileId != null && (uploadedFile == null || uploadedFile.Length == 0))
                    {
                        // check if the file is correct
                        var checkFileIdGuid = Guid.Parse(experience.FileId);
                        var checkFile = await _dbContext.View_ExperienceFiles
                            .Where(x => x.UserId == userIdGuid && x.FileId == checkFileIdGuid)
                            .FirstOrDefaultAsync();
                        if (checkFile == null)
                        {
                            throw new MyException("File is not correct or is not property of current user");
                        }

                        fileIdGuid = checkFileIdGuid;
                    }
                    else if (uploadedFile != null && uploadedFile.Length > 0)
                    {
                        var newFileId = Guid.NewGuid();
                        var newFileName = DateTimeOffset.Now.ToUnixTimeMilliseconds() + uploadedFile.FileName;

                        // insert the file into db
                        var fileNew = new DataModels.Files
                        {
                            Id = newFileId,
                            FileName = newFileName,
                            FileType = uploadedFile.ContentType,
                            FileCreated = DateTimeOffset.Now
                        };
                        await _dbContext.Files.AddAsync(fileNew);

                        fileIdGuid = newFileId;
                        fileName = newFileName;
                    }
                    else
                    {
                        //throw new MyException("file not selected");
                    }


                    // check and, in case, insert the place into db
                    if (experience.PlaceId != null && (experience.PlaceName == null || experience.PlaceName == ""))
                    {
                        placeIdGuid = Guid.Parse(experience.PlaceId);
                    }
                    else if (experience.PlaceName != null && experience.PlaceName != "")
                    {
                        var place = await _dbContext.Places.FirstOrDefaultAsync(x => x.Name == experience.PlaceName && x.Latitude == experience.PlaceLatitude && x.Longitude == experience.PlaceLongitude);
                        if (place != null)
                        {
                            placeIdGuid = place.Id;
                        }
                        else
                        {
                            var newPlaceId = Guid.NewGuid();

                            await _dbContext.Places.AddAsync(new DataModels.Places
                            {
                                Id = newPlaceId,
                                Name = experience.PlaceName,
                                Description = experience.PlaceDescription,
                                Latitude = experience.PlaceLatitude,
                                Longitude = experience.PlaceLongitude,
                            });

                            placeIdGuid = newPlaceId;
                        }
                    }
                    else
                    {
                        //throw new MyException("Place not selected");
                    }

                    // check, insert the experience into db
                    if (experience.Id != null)
                    {
                        experienceIdGuid = Guid.Parse(experience.Id);

                        var experienceUpdate = await _dbContext.Experiences.FirstOrDefaultAsync(x => x.UserId == userIdGuid && x.Id == experienceIdGuid);

                        // check if it has an old media, if there is delete it in the end
                        if (experienceUpdate.MediaFileId != null && experienceUpdate.MediaFileId != fileIdGuid)
                        {
                            oldFileId = experienceUpdate.MediaFileId;
                        }

                        experienceUpdate.Name = experience.Name;
                        experienceUpdate.Description = experience.Description;
                        experienceUpdate.Date = DateTimeHelpers.GetDateTimeFromDateFormatted(experience.Date);
                        experienceUpdate.Time = DateTimeHelpers.GetTimeSpanFromTimeFormatted(experience.Time);
                        experienceUpdate.MorningNightEnum = experience.MorningNightEnum;
                        experienceUpdate.PrivacyLevel = experience.PrivacyLevel;
                        experienceUpdate.MediaFileId = fileIdGuid;
                        experienceUpdate.PlaceId = placeIdGuid;
                        experienceUpdate.UserId = userIdGuid;
                        experienceUpdate.StatusEnum = experience.StatusEnum;

                        _dbContext.Experiences.Update(experienceUpdate);
                    }
                    else
                    {
                        experienceIdGuid = Guid.NewGuid();

                        var experienceNew = new DataModels.Experiences
                        {
                            Id = experienceIdGuid,
                            Name = experience.Name,
                            Description = experience.Description,
                            Date = DateTimeHelpers.GetDateTimeFromDateFormatted(experience.Date),
                            Time = DateTimeHelpers.GetTimeSpanFromTimeFormatted(experience.Time),
                            MorningNightEnum = experience.MorningNightEnum,
                            PrivacyLevel = experience.PrivacyLevel,
                            MediaFileId = fileIdGuid,
                            PlaceId = placeIdGuid,
                            UserId = userIdGuid,
                            DateTimeCreated = DateTimeOffset.Now,
                            StatusEnum = experience.StatusEnum
                        };

                        await _dbContext.Experiences.AddAsync(experienceNew);
                    }


                    await _dbContext.SaveChangesAsync();


                    // save file and delete old one
                    if (fileName != null)
                    {
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia);
                        await MediaEditor.SaveMedia(uploadedFile, folderPath, fileName);
                    }
                    if (oldFileId != null)
                    {
                        var oldMedia = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == oldFileId);
                        _dbContext.Files.Remove(oldMedia);
                        await _dbContext.SaveChangesAsync();

                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia);
                        MediaEditor.DeleteMedia(folderPath, oldMedia.FileName);
                    }


                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw e;
                }
            }
        }
        [HttpGet]
        [SessionAuthorize]
        public async Task<bool> DeleteExperience(string experienceId)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // delete experience from db
                    var userIdGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    var experienceIdGuid = Guid.Parse(experienceId);
                    var experience = await _dbContext.Experiences.FirstOrDefaultAsync(x => x.UserId == userIdGuid && x.Id == experienceIdGuid);
                    _dbContext.Experiences.Remove(experience);

                    // delete file from db
                    if (experience.MediaFileId != null)
                    {
                        var media = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == experience.MediaFileId);
                        _dbContext.Files.Remove(media);

                        // delete file from file system  
                        var mediaPath = Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, media.FileName);
                        var mediaPreviewPath = Path.Combine(Directory.GetCurrentDirectory(), Folder_ExperiencesMedia, "Previews", media.FileName);
                        if (System.IO.File.Exists(mediaPath))
                        {
                            System.IO.File.Delete(mediaPath);
                        }
                        if (System.IO.File.Exists(mediaPreviewPath))
                        {
                            System.IO.File.Delete(mediaPreviewPath);
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw e;
                }
            }
        }
        #endregion

    }
}
