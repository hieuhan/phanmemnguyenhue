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
    public class ActionController : Controller
    {
        // GET: Admin/Action
        [HttpGet]
        public async Task<ActionResult> Index(string keywords = "", byte statusId = 0, int page = 0, int pageSize = 200)
        {
            int orderByClauseId = 0;
            byte searchByDateType = 0, selfJoin = 0;

            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Actions actions = new Actions
            {
                ActBy = myPrincipal.UserName,
                Name = keywords,
                StatusId = statusId
            };

            var actionStatusTask = ActionStatus.Static_GetList();
            var actionsGetPageTask = actions.GetPage(DateTime.MinValue, DateTime.MinValue, searchByDateType,
                selfJoin,
                orderByClauseId, 0,
                page > 0 ? page - 1 : page, pageSize);

            await Task.WhenAll(actionStatusTask, actionsGetPageTask);

            ActionsVM model = new ActionsVM
            {
                ActionStatusList = actionStatusTask != null && actionStatusTask.Result.IsAny() ? actionStatusTask.Result : new List<ActionStatus>(),
                ActionsList = actionsGetPageTask.Result != null && actionsGetPageTask.Result.Item1.IsAny() ? actionsGetPageTask.Result.Item1 : null,
                Pagination = new PaginationVM(page, pageSize, actionsGetPageTask.Result.Item2)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> BindData(string keywords = "", byte statusId = 0, int page = 0, int pageSize = 200)
        {
            int orderByClauseId = 0;
            byte searchByDateType = 0, selfJoin = 0;

            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Actions actions = new Actions
            {
                ActBy = myPrincipal.UserName,
                Name = keywords,
                StatusId = statusId
            };

            var actionStatusTask = ActionStatus.Static_GetList();
            var actionsGetPageTask = actions.GetPage(DateTime.MinValue, DateTime.MinValue, searchByDateType,
                selfJoin,
                orderByClauseId, 0,
                page > 0 ? page - 1 : page, pageSize);

            await Task.WhenAll(actionStatusTask, actionsGetPageTask);

            ActionsVM model = new ActionsVM
            {
                ActionStatusList = actionStatusTask != null && actionStatusTask.Result.IsAny() ? actionStatusTask.Result : new List<ActionStatus>(),
                ActionsList = actionsGetPageTask.Result != null && actionsGetPageTask.Result.Item1.IsAny() ? actionsGetPageTask.Result.Item1 : null,
                Pagination = new PaginationVM(page, pageSize, actionsGetPageTask.Result.Item2)
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

            var parentActionTask = Actions.Static_GetListIsDisplayed();
            var actionStatusTask = ActionStatus.Static_GetList();
            await Task.WhenAll(parentActionTask, actionStatusTask);

            ActionEditVM model = new ActionEditVM
            {
                AddAnother = TempData["AddAnother"] != null,
                ParentActionsList = parentActionTask != null && parentActionTask.Result.IsAny() ? parentActionTask.Result : new List<Actions>(),
                ActionStatusList = actionStatusTask != null && actionStatusTask.Result.IsAny() ? actionStatusTask.Result : new List<ActionStatus>(),
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Add(ActionEditVM model)
        {
            if (ModelState.IsValid)
            {
                MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

                Actions actions = new Actions
                {
                    ActBy = myPrincipal.UserName,
                    Name = model.Name,
                    Path = model.Path,
                    Controller = model.Controller,
                    Action = model.Action
                };

                if (!string.IsNullOrWhiteSpace(model.Description))
                    actions.Description = model.Description;
                actions.ParentId = model.ParentId;
                actions.StatusId = model.StatusId;
                if (!string.IsNullOrWhiteSpace(model.IconPath))
                    actions.IconPath = model.IconPath;
                if (model.DisplayOrder.HasValue)
                    actions.DisplayOrder = model.DisplayOrder;
                actions.Display = model.Display;

                Tuple<string, string> tuple = await actions.Insert();

                if (tuple != null && !string.IsNullOrWhiteSpace(tuple.Item1) && tuple.Item1.Equals(ConstantHelper.ActionStatusSuccess))
                {
                    if (model.AddAnother)
                    {
                        return Json(new
                        {
                            Completed = true,
                            Message = tuple.Item2,
                            Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("$('#add-action-form').find('input[type=\"text\"]').val('');app.bindTableData('{0}')", Url.Action("BindData", "Action")))),
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            Completed = true,
                            Message = tuple.Item2,
                            Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("app.closeDialog('#_editForm');app.bindTableData('{0}')", Url.Action("BindData", "Action"))))
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
        public async Task<ActionResult> Edit(int actionId = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                return Redirect(Url.Action("NotFound", "Error"));
            }

            if (actionId <= 0)
            {
                return Redirect(Url.Action("NotFound", "Error"));
            }

            byte selfJoin = 1;
            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Actions actions = await Actions.Static_GetById(myPrincipal.UserName, actionId, selfJoin);

            if (actions == null || actions.ActionId <= 0)
            {
                return Redirect(Url.Action("NotFound", "Error"));
            }

            var parentActionTask = Actions.Static_GetListIsDisplayed();
            var actionStatusTask = ActionStatus.Static_GetList();
            await Task.WhenAll(parentActionTask, actionStatusTask);

            var model = new ActionEditVM
            {
                ParentActionsList = parentActionTask != null && parentActionTask.Result.IsAny() ? parentActionTask.Result : new List<Actions>(),
                ActionStatusList = actionStatusTask != null && actionStatusTask.Result.IsAny() ? actionStatusTask.Result : new List<ActionStatus>(),
                Name = actions.Name,
                Description = actions.Description.DefaultIfEmpty(),
                ParentId = actions.ParentId,
                Path = actions.Path,
                Controller = actions.Controller,
                Action = actions.Action,
                StatusId = actions.StatusId,
                IconPath = actions.IconPath.DefaultIfEmpty(),
                Display = actions.Display,
                DisplayOrder = actions.DisplayOrder
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(ActionEditVM model)
        {
            if (model.ActionId <= 0)
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

                var actions = await Actions.Static_GetById(myPrincipal.UserName, model.ActionId);

                if (actions == null || actions.ActionId <= 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new
                    {
                        Completed = false,
                        //ReturnUrl = Url.Action("NotFound", "Error")
                    }, JsonRequestBehavior.AllowGet);
                }

                actions.ActBy = myPrincipal.UserName;
                actions.Name = model.Name;
                actions.Description = model.Description.DefaultIfEmpty();
                actions.ParentId = model.ParentId;
                actions.Path = model.Path;
                actions.Controller = model.Controller;
                actions.Action = model.Action;
                actions.StatusId = model.StatusId;
                actions.IconPath = model.IconPath.DefaultIfEmpty();
                actions.Display = model.Display;
                actions.DisplayOrder = model.DisplayOrder;

                Tuple<string, string> tuple = await actions.Update();

                if (tuple != null && !string.IsNullOrWhiteSpace(tuple.Item1) && tuple.Item1.Equals(ConstantHelper.ActionStatusSuccess))
                {
                    return Json(new
                    {
                        Completed = true,
                        Message = tuple.Item2,
                        Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("app.closeDialog('#_editForm');app.bindTableData('{0}')", Url.Action("BindData", "Action"))))
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
        public async Task<JsonResult> Erase(int actionId = 0)
        {
            if (actionId <= 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new
                {
                    Completed = false,
                    //ReturnUrl = Url.Action("NotFound", "Error")
                }, JsonRequestBehavior.AllowGet);
            }

            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Actions actions = await Actions.Static_GetById(myPrincipal.UserName, actionId);

            if (actions == null || actions.ActionId <= 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new
                {
                    Completed = false,
                    //ReturnUrl = Url.Action("NotFound", "Error")
                }, JsonRequestBehavior.AllowGet);
            }

            actions.ActBy = myPrincipal.UserName;
            Tuple<string, string> tuple = await actions.Delete();

            if (tuple != null && !string.IsNullOrWhiteSpace(tuple.Item1) && tuple.Item1.Equals(ConstantHelper.ActionStatusSuccess))
            {
                return Json(new
                {
                    Completed = true,
                    Message = tuple.Item2,
                    Cb = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("app.closeDialog('#_message');app.bindTableData('{0}')", Url.Action("BindData", "Action"))))
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