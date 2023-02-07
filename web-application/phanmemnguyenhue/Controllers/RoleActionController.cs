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
    public class RoleActionController : Controller
    {
        // GET: Admin/RoleAction

        [HttpGet]
        public async Task<ActionResult> Index(string keywords = "", byte statusId = 0, int roleId = 0, int page = 0, int pageSize = 200)
        {
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

            Actions actions = new Actions
            {
                ActBy = myPrincipal.UserName,
                Name = keywords,
                RoleId = roleId,
                StatusId = statusId
            };

            byte selfJoin = 0;

            var actionStatusTask = ActionStatus.Static_GetList();
            var actionsGetPageTask = actions.GetPage(DateTime.MinValue, DateTime.MinValue, 0,
                selfJoin,
                0, 0,
                page > 0 ? page - 1 : page, pageSize);

            await Task.WhenAll(actionStatusTask, actionsGetPageTask);

            RoleActionVM model = new RoleActionVM
            {
                RoleId = roleId,
                Roles = roles,
                ActionStatusList = actionStatusTask != null && actionStatusTask.Result.IsAny() ? actionStatusTask.Result : new List<ActionStatus>(),
                ActionsList = actionsGetPageTask.Result != null && actionsGetPageTask.Result.Item1.IsAny() ? actionsGetPageTask.Result.Item1 : null,
                Pagination = new PaginationVM(page, pageSize, actionsGetPageTask.Result.Item2)
            };

            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> BindData(string keywords = "", byte statusId = 0, int roleId = 0, int page = 0, int pageSize = 200)
        {
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

            Actions actions = new Actions
            {
                ActBy = myPrincipal.UserName,
                Name = keywords,
                RoleId = roleId,
                StatusId = statusId
            };

            byte selfJoin = 0;

            var actionStatusTask = ActionStatus.Static_GetList();
            var actionsGetPageTask = actions.GetPage(DateTime.MinValue, DateTime.MinValue, 0,
                selfJoin,
                0, 0,
                page > 0 ? page - 1 : page, pageSize);

            await Task.WhenAll(actionStatusTask, actionsGetPageTask);

            RoleActionVM model = new RoleActionVM
            {
                RoleId = roleId,
                Roles = roles,
                ActionStatusList = actionStatusTask != null && actionStatusTask.Result.IsAny() ? actionStatusTask.Result : new List<ActionStatus>(),
                ActionsList = actionsGetPageTask.Result != null && actionsGetPageTask.Result.Item1.IsAny() ? actionsGetPageTask.Result.Item1 : null,
                Pagination = new PaginationVM(page, pageSize, actionsGetPageTask.Result.Item2)
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> MultipleAction(List<int> actionId, List<int> idRemove, int roleId = 0)
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

            if (actionId.IsAny() || idRemove.IsAny())
            {
                Tuple<string, string> tuple = await
                    RoleActions.Static_InsertMultiple(myPrincipal.UserName, roles.RoleId,
                        actionId.IsAny() ? string.Join(",", actionId) : string.Empty,
                        idRemove.IsAny() ? string.Join(",", idRemove) : string.Empty);

                if (tuple != null && !string.IsNullOrWhiteSpace(tuple.Item1) && tuple.Item1.Equals(ConstantHelper.ActionStatusSuccess))
                {
                    return Json(new
                    {
                        Completed = true,
                        Message = tuple.Item2,
                        Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes("app.closeDialog('#_editForm')"))
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
    }
}