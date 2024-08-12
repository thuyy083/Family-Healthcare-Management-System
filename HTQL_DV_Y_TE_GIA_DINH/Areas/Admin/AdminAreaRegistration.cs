using System.Web.Mvc;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", controller = "AdminHome", id = UrlParameter.Optional },
                namespaces: new string[] { "HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Controllers" }
            );
        }
    }
}