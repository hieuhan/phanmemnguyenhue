using Newtonsoft.Json;
using phanmemnguyenhue.helper;
using phanmemnguyenhue.library;
using phanmemnguyenhue.Models;
using phanmemnguyenhue.Services.Attributes;
using phanmemnguyenhue.Services.Extensions;
using phanmemnguyenhue.Services.Sercurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace phanmemnguyenhue.Controllers
{
    public class AccountController : Controller
    {
        // GET: Admin/Account
        public ActionResult Login(string returnUrl)
        {
            if (AppExtensions.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var model = new UserLoginVM
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UserLoginVM model)
        {
            if (ModelState.IsValid)
            {
                Users user = await Users.Static_GetByUserName(model.UserName);

                if (user != null && user.UserId > 0 && !string.IsNullOrWhiteSpace(user.Password))
                {
                    if (HashHelper.ValidatePassword(model.Password, user.Password))
                    {
                        var userSerializer = new UserSerializer
                        {
                            UserId = user.UserId,
                            UserName = user.UserName,
                            FullName = user.FullName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Avatar = user.Avatar
                        };

                        await Users.Static_Update_LastLoggedInAt(user.UserName, user.UserId);

                        string userData = JsonConvert.SerializeObject(userSerializer);

                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                            1,
                            user.UserName,
                            DateTime.Now,
                            model.RememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddHours(2),
                            model.RememberMe,
                            userData);

                        string encTicket = FormsAuthentication.Encrypt(authTicket);

                        FormsAuthentication.SetAuthCookie(authTicket.Name, true);

                        HttpCookie formsAuthenticationTicketCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
                        {
                            Domain = FormsAuthentication.CookieDomain,
                            Path = FormsAuthentication.FormsCookiePath,
                            HttpOnly = true,
                            Secure = FormsAuthentication.RequireSSL,
                            Expires = authTicket.Expiration
                        };

                        Response.Cookies.Add(formsAuthenticationTicketCookie);

                        if (Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl.Length > 1 &&
                                model.ReturnUrl.StartsWith("/") && !model.ReturnUrl.StartsWith("//") &&
                                !model.ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        return RedirectToAction("Index", "Dashboard");
                    }
                }

                this.ToastrError("Thông báo", "Thông tin đăng nhập không chính xác.");
            }

            return View(model);
        }

        [HttpGet]
        [AuthorizeRole]
        public new async Task<ActionResult> Profile()
        {
            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Users currentUser = await Users.Static_GetById(myPrincipal.UserName, myPrincipal.UserId);

            if (currentUser == null || currentUser.UserId <= 0)
            {
                return Redirect(Url.Action("NotFound", "Error"));
            }

            List<Genders> gendersList = await Genders.Static_GetList();

            var model = new UserProfileVM
            {
                GendersList = gendersList.IsAny() ? gendersList : new List<Genders>()
            };

            model.CopyPropertiesFrom(currentUser);

            if (currentUser.BirthDay != DateTime.MinValue)
            {
                model.Day = currentUser.BirthDay.Day;
                model.Month = currentUser.BirthDay.Month;
                model.Year = currentUser.BirthDay.Year;
            }

            return View(model);
        }

        [HttpPost]
        [AuthorizeRole]
        [ValidateAntiForgeryToken]
        public new async Task<ActionResult> Profile(UserProfileVM model)
        {
            if (ModelState.IsValid)
            {
                MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

                Users currentUser = await Users.Static_GetById(myPrincipal.UserName, myPrincipal.UserId);

                if (currentUser == null || currentUser.UserId <= 0)
                {
                    return RedirectToAction("NotFound", "Error");
                }

                currentUser.ActBy = myPrincipal.UserName;
                currentUser.FullName = model.FullName;
                currentUser.Email = model.Email.DefaultIfEmpty();
                currentUser.PhoneNumber = model.PhoneNumber.DefaultIfEmpty();
                currentUser.GenderId = model.GenderId;
                currentUser.Address = model.Address.DefaultIfEmpty();

                if (!model.RemoveImage)
                {
                    currentUser.Avatar = model.Avatar.DefaultIfEmpty();
                }

                string rootDir = Request.PhysicalApplicationPath,
                    filePath = string.Empty;

                if (model.PostedFile != null)
                {
                    filePath = FileUploadHelper.SaveFile(model.PostedFile,
                        rootDir, ConstantHelper.MediaPath, true);
                }

                if (!string.IsNullOrEmpty(filePath))
                {
                    currentUser.Avatar = filePath.Replace("\\", "/");
                }

                if (model.Day > 0 && model.Month > 0 && model.Year > 0)
                {
                    currentUser.BirthDay = new DateTime(model.Year, model.Month, model.Day);
                }

                Tuple<string, string> userProfileUpdateResult = await currentUser.Update();

                if (userProfileUpdateResult.Item1.Equals(ConstantHelper.ActionStatusSuccess))
                {
                    var userSerializer = new UserSerializer
                    {
                        UserId = currentUser.UserId,
                        UserName = currentUser.UserName,
                        FullName = currentUser.FullName.DefaultIfEmpty(),
                        Email = currentUser.Email,
                        PhoneNumber = currentUser.PhoneNumber,
                        Avatar = currentUser.Avatar
                    };

                    string userData = JsonConvert.SerializeObject(userSerializer);

                    HttpCookie cookie = FormsAuthentication.GetAuthCookie(FormsAuthentication.FormsCookieName, true);
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                    if (ticket != null)
                    {
                        var newticket = new FormsAuthenticationTicket
                        (
                            ticket.Version,
                            ticket.Name,
                            ticket.IssueDate,
                            ticket.Expiration,
                            true,
                            userData,
                            ticket.CookiePath
                        );

                        FormsAuthentication.SetAuthCookie(ticket.Name, true);

                        cookie.Value = FormsAuthentication.Encrypt(newticket);
                        cookie.Expires = newticket.Expiration;
                        Response.Cookies.Set(cookie);
                    }

                    this.ToastrSuccess("Thông báo", userProfileUpdateResult.Item2);
                }
                else
                {
                    this.ToastrError("Thông báo", userProfileUpdateResult.Item2);
                }

                return RedirectToAction("Profile", "Account");
            }

            List<Genders> gendersList = await Genders.Static_GetList();

            model.GendersList = gendersList.IsAny() ? gendersList : new List<Genders>();

            return View(model);
        }

        [HttpGet]
        [AuthorizeRole]
        public ActionResult ChangePassword()
        {
            return View(new UserChangePasswordVM());
        }

        [HttpPost]
        [AuthorizeRole]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(UserChangePasswordVM model)
        {
            if (ModelState.IsValid)
            {
                MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

                Users users = await Users.Static_GetById(myPrincipal.UserName, myPrincipal.UserId);

                if (users != null && users.UserId > 0)
                {
                    if (users.UserStatusId == ConstantHelper.UserStatusIdApproved)
                    {
                        if (HashHelper.ValidatePassword(model.Password, users.Password))
                        {
                            users.ActBy = myPrincipal.UserName;
                            users.Password = HashHelper.HashPassword(model.NewPassword);

                            Tuple<string, string> changePasswordResult = await users.ChangePassword();

                            if (changePasswordResult.Item1.Equals(ConstantHelper.ActionStatusSuccess))
                            {

                                this.ToastrSuccess("Thông báo", "Đổi mật khẩu thành công.");

                                return RedirectToAction("Login", "Account");
                            }
                            else
                            {
                                this.ToastrError("Thông báo", changePasswordResult.Item2);
                            }
                        }
                        else this.ToastrError("Thông báo", "Mật khẩu hiện tại không chính xác.");
                    }
                    else this.ToastrError("Thông báo", "Tài khoản chưa được kích hoạt.Vui lòng liên hệ với Quản trị hệ thống để được trợ giúp.");
                }
                else this.ToastrError("Thông báo", "Tài khoản không tồn tại trên hệ thống.");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty)
            {
                Expires = DateTime.Now.AddYears(-1)
            };

            Response.Cookies.Add(cookie);

            return RedirectToAction("Login", "Account");
        }
    }
}