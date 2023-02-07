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
using System.Web.Mvc;

namespace phanmemnguyenhue.Controllers
{
    [AuthorizeRole]
    public class RoleController : Controller
    {
        // GET: Admin/Role

        [HttpGet]
        public async Task<ActionResult> Index(string keywords = "", byte statusId = 0, int page = 0, int pageSize = 200)
        {
            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Roles roles = new Roles
            {
                ActBy = myPrincipal.UserName,
                Name = keywords,
                StatusId = statusId
            };

            var statusTask = Status.Static_GetList();
            var rolesGetPageTask = roles.GetPage(DateTime.MinValue, DateTime.MinValue, 0, 0, page > 0 ? page - 1 : page, pageSize);

            await Task.WhenAll(statusTask, rolesGetPageTask);

            RoleVM model = new RoleVM
            {
                StatusList = statusTask != null && statusTask.Result.IsAny() ? statusTask.Result : new List<Status>(),
                RolesList = rolesGetPageTask.Result != null && rolesGetPageTask.Result.Item1.IsAny() ? rolesGetPageTask.Result.Item1 : null,
                Pagination = new PaginationVM(page, pageSize, rolesGetPageTask.Result.Item2)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> BindData(string keywords = "", byte statusId = 0, int page = 0, int pageSize = 200)
        {
            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Roles roles = new Roles
            {
                ActBy = myPrincipal.UserName,
                Name = keywords,
                StatusId = statusId
            };

            var statusTask = Status.Static_GetList();
            var rolesGetPageTask = roles.GetPage(DateTime.MinValue, DateTime.MinValue, 0, 0, page > 0 ? page - 1 : page, pageSize);

            await Task.WhenAll(statusTask, rolesGetPageTask);

            RoleVM model = new RoleVM
            {
                StatusList = statusTask != null && statusTask.Result.IsAny() ? statusTask.Result : new List<Status>(),
                RolesList = rolesGetPageTask.Result != null && rolesGetPageTask.Result.Item1.IsAny() ? rolesGetPageTask.Result.Item1 : null,
                Pagination = new PaginationVM(page, pageSize, rolesGetPageTask.Result.Item2)
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

            List<Status> statusList = await Status.Static_GetList();

            RoleEditVM model = new RoleEditVM
            {
                StatusList = statusList.IsAny() ? statusList : new List<Status>()
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Add(RoleEditVM model)
        {
            if (ModelState.IsValid)
            {
                MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

                Roles roles = new Roles
                {
                    ActBy = myPrincipal.UserName,
                    Name = model.Name
                };

                if (!string.IsNullOrWhiteSpace(model.Description))
                    roles.Description = model.Description;
                roles.StatusId = model.StatusId;
                if (model.DisplayOrder.HasValue)
                    roles.DisplayOrder = model.DisplayOrder;

                Tuple<string, string> tuple = await roles.Insert();

                if (tuple != null && !string.IsNullOrWhiteSpace(tuple.Item1) && tuple.Item1.Equals(ConstantHelper.ActionStatusSuccess))
                {
                    if (model.AddAnother)
                    {
                        return Json(new
                        {
                            Completed = true,
                            Message = tuple.Item2,
                            Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("$('#add-role-form').find('input[type=\"text\"]').val('');app.bindTableData('{0}')", Url.Action("BindData", "Role"))))
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            Completed = true,
                            Message = tuple.Item2,
                            Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("app.closeDialog('#_editForm');app.bindTableData('{0}')", Url.Action("BindData", "Role"))))
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
        public async Task<ActionResult> Edit(int roleId = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                return Redirect(Url.Action("NotFound", "Error"));
            }

            if (roleId <= 0)
            {
                return Redirect(Url.Action("NotFound", "Error"));
            }

            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Roles roles = await Roles.Static_GetById(myPrincipal.UserName, roleId);

            if (roles == null || roles.RoleId <= 0)
            {
                return Redirect(Url.Action("NotFound", "Error"));
            }

            if (roles.BuildIn == 1)
            {
                return Redirect(Url.Action("AccessDenied", "Error"));
            }

            List<Status> statusList = await Status.Static_GetList();

            var model = new RoleEditVM
            {
                Name = roles.Name,
                Description = roles.Description.DefaultIfEmpty(),
                StatusId = roles.StatusId,
                DisplayOrder = roles.DisplayOrder,
                StatusList = statusList.IsAny() ? statusList : new List<Status>()
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(RoleEditVM model)
        {
            if (model.RoleId <= 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new
                {
                    Completed = false,
                    //ReturnUrl = Url.Action("NotFound", "Error")
                }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

                Roles roles = await Roles.Static_GetById(myPrincipal.UserName, model.RoleId);

                if (roles == null || roles.RoleId <= 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new
                    {
                        Completed = false,
                        //ReturnUrl = Url.Action("NotFound", "Error")
                    }, JsonRequestBehavior.AllowGet);
                }

                if (roles.BuildIn == 1)
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Json(new
                    {
                        Completed = false,
                        //ReturnUrl = Url.Action("NotFound", "Error")
                    }, JsonRequestBehavior.AllowGet);
                }

                roles.ActBy = myPrincipal.UserName;
                roles.Name = model.Name;
                roles.Description = model.Description.DefaultIfEmpty();
                roles.StatusId = model.StatusId;
                roles.DisplayOrder = model.DisplayOrder;

                Tuple<string, string> tuple = await roles.Update();

                if (tuple != null && !string.IsNullOrWhiteSpace(tuple.Item1) && tuple.Item1.Equals(ConstantHelper.ActionStatusSuccess))
                {
                    return Json(new
                    {
                        Completed = true,
                        Message = tuple.Item2,
                        Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("app.closeDialog('#_editForm');app.bindTableData('{0}')", Url.Action("BindData", "Role"))))
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

        [HttpGet]
        public async Task<JsonResult> Erase(int roleId = 0)
        {
            if (roleId <= 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new
                {
                    Completed = false,
                    //ReturnUrl = Url.Action("NotFound", "Error")
                }, JsonRequestBehavior.AllowGet);
            }

            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Roles roles = await Roles.Static_GetById(myPrincipal.UserName, roleId);

            if (roles == null || roles.RoleId <= 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new
                {
                    Completed = false,
                    //ReturnUrl = Url.Action("NotFound", "Error")
                }, JsonRequestBehavior.AllowGet);
            }

            if (roles.BuildIn == 1)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new
                {
                    Completed = false,
                    //ReturnUrl = Url.Action("NotFound", "Error")
                }, JsonRequestBehavior.AllowGet);
            }

            roles.ActBy = myPrincipal.UserName;
            Tuple<string, string> tuple = await roles.Delete();

            if (tuple != null && !string.IsNullOrWhiteSpace(tuple.Item1) && tuple.Item1.Equals(ConstantHelper.ActionStatusSuccess))
            {
                return Json(new
                {
                    Completed = true,
                    Message = tuple.Item2,
                    Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("app.closeDialog('#_message');app.bindTableData('{0}')", Url.Action("BindData", "Role"))))
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