using phanmemnguyenhue.Services.Sercurity;
using System.Web;

namespace phanmemnguyenhue.Services.Extensions
{
    public static class AppExtensions
    {
        public static bool IsAuthenticated
        {
            get
            {
                return HttpContext.Current != null && HttpContext.Current.User != null &&
                       HttpContext.Current.User.Identity != null &&
                       HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }

        public static MyPrincipal GetCurrentUser()
        {
            return IsAuthenticated ? ((MyPrincipal)HttpContext.Current.User) : null;
        }
    }
}