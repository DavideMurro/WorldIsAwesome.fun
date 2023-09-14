using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using www.worldisawesome.fun.DBContext;
using www.worldisawesome.fun.Models;
using www.worldisawesome.fun.Services;

namespace www.worldisawesome.fun.Controllers
{
    [Route("{controller=Home}/{action=Index}/{id?}")]
    public class StatusController : Controller
    {
        private readonly WorldIsAwesomeContext _dbContext;


        public StatusController(
            WorldIsAwesomeContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> ActivateUser(Guid userId, string activationCode)
        {
            try
            {
                var user = await _dbContext.Users
                    .Where(x => x.Id == userId)
                    .FirstOrDefaultAsync();

                ViewBag.HeaderTitle = "Activate user";

                if (user == null)
                {
                    throw new Exception("User doesn t exist");
                }

                if (user.StatusEnum == UserStatusEnum.Activated)
                {
                    throw new Exception("User already activated");
                }

                if (!SecurePasswordHasher.Verify(user.Mail, activationCode))
                {
                    throw new Exception("Wrong activation code");
                }

                user.StatusEnum = UserStatusEnum.Activated;
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();


                // save session
                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(ClaimTypes.Name, user.Nickname),
                                //new Claim(ClaimTypes.Role, "Administrator")
                            };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                    IsPersistent = false
                };
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);


                ViewBag.Result = true;
                ViewBag.User = user;
                ViewBag.ResultMessage = "";
                //ViewBag.resultMessage = "User Activation completed. Welcome <b>" + user.Nickname + "</b>";

                return View();
            }
            catch (Exception e)
            {
                ViewBag.Result = false;
                ViewBag.ResultMessage = e.Message;

                return View();
                // throw e;
            }
        }

    }
}