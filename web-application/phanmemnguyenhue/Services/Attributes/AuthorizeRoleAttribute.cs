using phanmemnguyenhue.Services.Sercurity;
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace phanmemnguyenhue.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        protected virtual MyPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as MyPrincipal; }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            var user = httpContext.User as MyPrincipal;

            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return false;
            }

            var routeData = httpContext.Request.RequestContext.RouteData;
            string currentAction = routeData.GetRequiredString("action"),
            currentController = routeData.GetRequiredString("controller");

            string path = string.Format("{0}/{1}", currentController, currentAction).ToLower();


            if (user.Identity.IsAuthenticated)
            {
                if (library.Users.Static_HasPriv(user.UserId, path) != 1)
                {
                    return false;
                }
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var url = new UrlHelper(filterContext.RequestContext);
            var accessDeniedUrl = url.Action("AccessDenied", "Error");

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            Completed = false,
                            Message = "Bạn không có quyền truy cập nội dung này.",
                            ReturnUrl = accessDeniedUrl
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(
                                new
                                {
                                    controller = "Error",
                                    action = "AccessDenied"
                                }));
                }
            }
            else
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            Completed = false,
                            ReturnUrl = filterContext.HttpContext.Request.UrlReferrer != null && filterContext.HttpContext.Request.UrlReferrer.PathAndQuery.IndexOf("login.html", StringComparison.Ordinal) < 0 ? string.Format("{0}?ReturnUrl={1}", FormsAuthentication.LoginUrl, filterContext.HttpContext.Request.UrlReferrer.PathAndQuery) : string.Concat(FormsAuthentication.LoginUrl, "?ReturnUrl=/")
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                    filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
                    filterContext.HttpContext.Response.End();
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Account",
                                action = "Login",
                                //
                                returnUrl = request.Url?.GetComponents(UriComponents.PathAndQuery,
                                    UriFormat.SafeUnescaped)
                            }));
                }
            }
        }
    }
}