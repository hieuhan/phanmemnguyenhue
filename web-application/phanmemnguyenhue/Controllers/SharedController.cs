using phanmemnguyenhue.helper;
using phanmemnguyenhue.library;
using phanmemnguyenhue.Models;
using phanmemnguyenhue.Services.Extensions;
using phanmemnguyenhue.Services.Sercurity;
using System.Collections.Generic;
using System.Web.Mvc;

namespace phanmemnguyenhue.Controllers
{
    public class SharedController : Controller
    {
        // GET: Shared
        [ChildActionOnly]
        public ActionResult PartialHeader()
        {
            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            List<Actions> actionsListByUser = Actions.Static_GetByUser(myPrincipal.UserName, myPrincipal.UserId);

            var model = new HeaderVM
            {
                ActionsList = new List<Actions>(),
                ParentActionsList = new List<Actions>()
            };

            if (actionsListByUser.IsAny())
            {
                foreach (var item in actionsListByUser)
                {
                    if (item.ParentId == 0)
                    {
                        model.ParentActionsList.Add(item);
                    }
                    else model.ActionsList.Add(item);
                }
            }

            return PartialView(model);
        }
    }
}