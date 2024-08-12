using System.Web.Mvc;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.BenhNhan
{
    public class BenhNhanAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BenhNhan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BenhNhan_default",
                "BenhNhan/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}