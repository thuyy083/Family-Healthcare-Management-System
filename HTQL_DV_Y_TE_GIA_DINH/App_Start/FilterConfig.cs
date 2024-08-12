using System.Web;
using System.Web.Mvc;

namespace HTQL_DV_Y_TE_GIA_DINH
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
