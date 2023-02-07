using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace phanmemnguyenhue
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            #region User

            routes.MapRoute(
                "User",
                "user.html",
                new { controller = "User", action = "Index" }
            );

            routes.MapRoute(
                "AddUser",
                "user/add.html",
                new { controller = "User", action = "Add" }
            );

            routes.MapRoute(
                "EditUser",
                "user/edit/{userId}.html",
                new { controller = "User", action = "Edit", userId = UrlParameter.Optional },
                new { userId = @"\d+" }
            );

            routes.MapRoute(
                "EraseUser",
                "user/del/{userId}.html",
                new { controller = "User", action = "Erase", userId = UrlParameter.Optional },
                new { userId = @"\d+" }
            );

            #endregion

            #region Role

            routes.MapRoute(
                "Role",
                "role/list.html",
                new { action = "Index", Controller = "Role" }
            );

            routes.MapRoute(
                "AddRole",
                "role/add.html",
                new { action = "Add", Controller = "Role" }
            );

            routes.MapRoute(
                "EditRole",
                "role/edit/{roleId}.html",
                new { action = "Edit", Controller = "Role", roleId = UrlParameter.Optional },
                new { roleId = @"\d+" }
            );

            routes.MapRoute(
                "EraseRole",
                "role/del/{roleId}.html",
                new { action = "Erase", Controller = "Role", roleId = UrlParameter.Optional },
                new { roleId = @"\d+" }
            );

            routes.MapRoute(
                "RoleAction",
                "role/assign/{roleId}.html",
                new { action = "Index", Controller = "RoleAction", roleId = UrlParameter.Optional },
                new { roleId = @"\d+" }
            );

            #endregion

            #region Action

            routes.MapRoute(
                "Action",
                "action.html",
                new { controller = "Action", action = "Index" }
            );

            routes.MapRoute(
                "AddAction",
                "action/add.html",
                new { controller = "Action", action = "Add" }
            );

            routes.MapRoute(
                "EditAction",
                "action/edit/{actionId}.html",
                new { controller = "Action", action = "Edit", actionId = UrlParameter.Optional },
                new { actionId = @"\d+" }
            );

            routes.MapRoute(
                "EraseAction",
                "action/del/{actionId}.html",
                new { controller = "Action", action = "Erase", actionId = UrlParameter.Optional },
                new { actionId = @"\d+" }
            );

            #endregion

            #region Ajax

            routes.MapRoute(
                "Ajax_GetDistrictsBy",
                "api/district/get.html",
                new { action = "GetDistrictsBy", Controller = "Ajax" }
            );

            routes.MapRoute(
                "Ajax_GetWardsBy",
                "api/ward/get.html",
                new { action = "GetWardsBy", Controller = "Ajax" }
            );

            routes.MapRoute(
                "Ajax_GetStreetsBy",
                "api/street/get.html",
                new { action = "GetStreetsBy", Controller = "Ajax" }
            );

            routes.MapRoute(
               "Ajax_GetProjectsBy",
               "api/project/get.html",
               new { action = "GetProjectsBy", Controller = "Ajax" }
           );

            #endregion

            routes.MapRoute(
               "Login",
               "login.html",
               new { controller = "Account", action = "Login" }
            );

            routes.MapRoute(
               "Profile",
               "profile.html",
               new { controller = "Account", action = "Profile" }
            );

            routes.MapRoute(
               "ChangePassword",
               "change-password.html",
               new { controller = "Account", action = "ChangePassword" }
            );

            routes.MapRoute(
               "Logout",
               "logout.html",
               new { controller = "Account", action = "Logout" }
            );

            routes.MapRoute(
              "NotFound",
              "404.html",
              new { controller = "Error", action = "NotFound" }
            );

            routes.MapRoute(
               "AccessDenied",
               "403.html",
               new { controller = "Error", action = "AccessDenied" }
            );

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
