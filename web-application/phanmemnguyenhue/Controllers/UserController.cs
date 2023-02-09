using phanmemnguyenhue.helper;
using phanmemnguyenhue.library;
using phanmemnguyenhue.Models;
using phanmemnguyenhue.Services.Attributes;
using phanmemnguyenhue.Services.Extensions;
using phanmemnguyenhue.Services.Sercurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace phanmemnguyenhue.Controllers
{
    [AuthorizeRole]
    public class UserController : Controller
    {
        // GET: Admin/User
        [HttpGet]
        public async Task<ActionResult> Index(string keywords = "", byte userStatusId = 0, int page = 0, int pageSize = 200)
        {
            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Users users = new Users
            {
                ActBy = myPrincipal.UserName,
                UserStatusId = userStatusId
            };

            keywords = keywords.StripTags();
            var userStatusTask = UserStatus.Static_GetList();
            var usersGetPageTask = users.GetPage(keywords, DateTime.MinValue, DateTime.MinValue, 0, 0, page > 0 ? page - 1 : page, pageSize);

            await Task.WhenAll(userStatusTask, usersGetPageTask);

            UserVM model = new UserVM
            {
                UserStatusList = userStatusTask != null && userStatusTask.Result.IsAny() ? userStatusTask.Result : new List<UserStatus>(),
                UsersList = usersGetPageTask.Result != null && usersGetPageTask.Result.Item1.IsAny() ? usersGetPageTask.Result.Item1 : null,
                Pagination = new PaginationVM(page, pageSize, usersGetPageTask.Result.Item2)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> BindData(string keywords = "", byte statusId = 0, int page = 0, int pageSize = 200)
        {
            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Users users = new Users
            {
                ActBy = myPrincipal.UserName,
                UserStatusId = statusId
            };

            keywords = keywords.StripTags();
            var userStatusTask = UserStatus.Static_GetList();
            var usersGetPageTask = users.GetPage(keywords, DateTime.MinValue, DateTime.MinValue, 0, 0, page > 0 ? page - 1 : page, pageSize);

            await Task.WhenAll(userStatusTask, usersGetPageTask);

            UserVM model = new UserVM
            {
                UserStatusList = userStatusTask != null && userStatusTask.Result.IsAny() ? userStatusTask.Result : new List<UserStatus>(),
                UsersList = usersGetPageTask.Result != null && usersGetPageTask.Result.Item1.IsAny() ? usersGetPageTask.Result.Item1 : null,
                Pagination = new PaginationVM(page, pageSize, usersGetPageTask.Result.Item2)
            };

            return PartialView(model);
        }

        [HttpGet]
        public async Task<ActionResult> Add()
        {
            if (!Request.IsAjaxRequest())
            {
                return Redirect(Url.Action("NotFound", "Error"));
            }

            var gendersTask = Genders.Static_GetList();
            var userStatusTask = UserStatus.Static_GetList();
            var rolesTask = Roles.Static_GetList();
            var sitesTask = Sites.Static_GetList();

            await Task.WhenAll(gendersTask, userStatusTask, rolesTask, sitesTask);

            UserAddVM model = new UserAddVM
            {
                UserStatusList = userStatusTask != null && userStatusTask.Result.IsAny() ? userStatusTask.Result : new List<UserStatus>(),
                GendersList = gendersTask != null && gendersTask.Result.IsAny() ? gendersTask.Result : new List<Genders>(),
                RolesList = rolesTask != null && rolesTask.Result.IsAny() ? rolesTask.Result : new List<Roles>(),
                SitesList = sitesTask != null && sitesTask.Result.IsAny() ? sitesTask.Result : new List<Sites>()
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Add(UserAddVM model)
        {
            if (ModelState.IsValid)
            {
                MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

                Users users = new Users
                {
                    ActBy = myPrincipal.UserName
                };

                users.CopyPropertiesFrom(model);
                users.Password = HashHelper.HashPassword(model.Password);

                if (!model.RemoveImage)
                {
                    users.Avatar = model.Avatar.DefaultIfEmpty();
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
                    users.Avatar = filePath.Replace("\\", "/");
                }

                if (model.Day > 0 && model.Month > 0 && model.Year > 0)
                {
                    users.BirthDay = new DateTime(model.Year, model.Month, model.Day);
                }

                Tuple<string, string> tuple = await users.Insert();

                if (tuple != null && !string.IsNullOrWhiteSpace(tuple.Item1) && tuple.Item1.Equals(ConstantHelper.ActionStatusSuccess))
                {
                    if (model.RoleIds.IsAny())
                    {
                        await UserRoles.Static_InsertMultiple(myPrincipal.UserName, users.UserId, string.Join(",", model.RoleIds));
                    }

                    if (model.SiteIds.IsAny())
                    {
                        await UserSites.Static_InsertMultiple(myPrincipal.UserName, users.UserId, string.Join(",", model.SiteIds));
                    }

                    if (model.AddAnother)
                    {
                        return Json(new
                        {
                            Completed = true,
                            Message = tuple.Item2,
                            Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("$(':input','#add-user-form') .not(':button, :submit, :reset, :hidden').val('').prop('checked', false).prop('selected', false);$('#AvatarImg','#add-user-form').attr('src', '/assets/images/avatar/default.png');app.bindTableData('{0}')", Url.Action("BindData", "User"))))
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            Completed = true,
                            Message = tuple.Item2,
                            Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("app.closeDialog('#_editForm');app.bindTableData('{0}')", Url.Action("BindData", "User"))))
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        Completed = true,
                        Message = tuple.Item2,
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new
            {
                Completed = false,
                Message = "Quý khách vui lòng thử lại sau."
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int userId = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                return Redirect("/404.html");
            }

            if (userId <= 0)
            {
                return Redirect("/404.html");
            }

            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Users users = await Users.Static_GetById(myPrincipal.UserName, userId);

            if (users == null || users.UserId <= 0)
            {
                return Redirect("/404.html");
            }

            if (users.BuildIn == 1)
            {
                return Redirect("/403.html");
            }

            var gendersTask = Genders.Static_GetList();
            var userStatusTask = UserStatus.Static_GetList();
            var rolesTask = Roles.Static_GetListBy(myPrincipal.UserName, users.UserId);
            var sitesTask = Sites.Static_GetListBy(myPrincipal.UserName, users.UserId);

            await Task.WhenAll(gendersTask, userStatusTask, rolesTask, sitesTask);

            var model = new UserEditVM
            {
                Users = users,
                RolesList = rolesTask != null && rolesTask.Result.IsAny() ? rolesTask.Result : new List<Roles>(),
                SitesList = sitesTask != null && sitesTask.Result.IsAny() ? sitesTask.Result : new List<Sites>(),
                GendersList = gendersTask != null && gendersTask.Result.IsAny() ? gendersTask.Result : new List<Genders>(),
                UserStatusList = userStatusTask != null && userStatusTask.Result.IsAny() ? userStatusTask.Result : new List<UserStatus>()
            };

            model.CopyPropertiesFrom(users);

            if (users.BirthDay != DateTime.MinValue)
            {
                model.Day = users.BirthDay.Day;
                model.Month = users.BirthDay.Month;
                model.Year = users.BirthDay.Year;
            }

            return PartialView(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(UserEditVM model)
        {
            if (model.UserId <= 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new
                {
                    Completed = false,
                    //ReturnUrl = Url.Action("NotFound", "Error")
                }, JsonRequestBehavior.AllowGet);
            }

            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            if (ModelState.IsValid)
            {
                model.Users = await Users.Static_GetById(myPrincipal.UserName, model.UserId);

                if (model.Users == null || model.Users.UserId <= 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new
                    {
                        Completed = false,
                        //ReturnUrl = Url.Action("NotFound", "Error")
                    }, JsonRequestBehavior.AllowGet);
                }

                if (model.Users.BuildIn == 1)
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Json(new
                    {
                        Completed = false,
                        //ReturnUrl = Url.Action("AccessDenied", "Error")
                    }, JsonRequestBehavior.AllowGet);
                }

                model.Users.FullName = model.FullName;
                model.Users.Email = model.Email.DefaultIfEmpty();
                model.Users.PhoneNumber = model.PhoneNumber.DefaultIfEmpty();
                model.Users.GenderId = model.GenderId;
                model.Users.Address = model.Address.DefaultIfEmpty();
                if (model.Users.BuildIn <= 0)
                    model.Users.UserStatusId = model.UserStatusId;

                if (!model.RemoveImage)
                {
                    model.Users.Avatar = model.Avatar.DefaultIfEmpty();
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
                    model.Users.Avatar = filePath.Replace("\\", "/");
                }

                if (model.Day > 0 && model.Month > 0 && model.Year > 0)
                {
                    model.Users.BirthDay = new DateTime(model.Year, model.Month, model.Day);
                }

                Tuple<string, string> tuple = await model.Users.Update();

                if (tuple != null && !string.IsNullOrWhiteSpace(tuple.Item1) && tuple.Item1.Equals(ConstantHelper.ActionStatusSuccess))
                {
                    var userRolesInsertTask = UserRoles.Static_InsertMultiple(myPrincipal.UserName, model.Users.UserId, model.RoleIds.IsAny() ? string.Join(",", model.RoleIds) : string.Empty,
                        model.RoleIdsRemove.IsAny() ? string.Join(",", model.RoleIdsRemove) : string.Empty);

                    var userSitesInsertTask = UserSites.Static_InsertMultiple(myPrincipal.UserName, model.Users.UserId, model.SiteIds.IsAny() ? string.Join(",", model.SiteIds) : string.Empty,
                        model.SiteIdsRemove.IsAny() ? string.Join(",", model.SiteIdsRemove) : string.Empty);

                    await Task.WhenAll(userRolesInsertTask, userSitesInsertTask);

                    return Json(new
                    {
                        Completed = true,
                        Message = tuple.Item2,
                        Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("app.closeDialog('#_editForm');app.bindTableData('{0}')", Url.Action("BindData", "User"))))
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Completed = false,
                        Message = tuple.Item2
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new
            {
                Completed = false,
                Message = "Quý khách vui lòng thử lại sau."
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Erase(int UserId = 0)
        {
            if (UserId <= 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new
                {
                    Completed = false,
                    //ReturnUrl = Url.Action("NotFound", "Error")
                }, JsonRequestBehavior.AllowGet);
            }

            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Users Users = await Users.Static_GetById(myPrincipal.UserName, UserId);

            if (Users == null || Users.UserId <= 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new
                {
                    Completed = false,
                    //ReturnUrl = Url.Action("NotFound", "Error")
                }, JsonRequestBehavior.AllowGet);
            }

            if (Users.BuildIn == 1)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new
                {
                    Completed = false,
                    //ReturnUrl = Url.Action("AccessDenied", "Error")
                }, JsonRequestBehavior.AllowGet);
            }

            Users.ActBy = myPrincipal.UserName;
            Tuple<string, string> tuple = await Users.Delete();

            if (tuple != null && !string.IsNullOrWhiteSpace(tuple.Item1) && tuple.Item1.Equals(ConstantHelper.ActionStatusSuccess))
            {
                return Json(new
                {
                    Completed = true,
                    Message = tuple.Item2,
                    Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("app.closeDialog('#_message');app.bindTableData('{0}')", Url.Action("BindData", "User"))))
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Completed = false,
                    Message = tuple.Item2
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}