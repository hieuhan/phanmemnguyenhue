using phanmemnguyenhue.Services.Attributes;
using System.Web.Mvc;

namespace phanmemnguyenhue
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MyAuthTicketAttribute());
        }
    }
}
