using System.Web.Mvc;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.BacSi
{
    public class BacSiAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BacSi";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BacSi_default",
                "BacSi/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}