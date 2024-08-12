using System.Web.Mvc;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.NhanVien
{
    public class NhanVienAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "NhanVien";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "NhanVien_default",
                "NhanVien/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}