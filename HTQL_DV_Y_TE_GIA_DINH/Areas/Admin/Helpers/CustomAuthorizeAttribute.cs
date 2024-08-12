using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (httpContext.Session["TaiKhoanDangNhap"] == null)
            {
                return false; // Trả về false nếu chưa đăng nhập
            }
            else
            {
                return true; // Trả về true nếu đã đăng nhập
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Chuyển hướng đến trang đăng nhập khi người dùng không có quyền truy cập
            filterContext.Result = new RedirectResult("~/Home/DangNhap");
        }
    }
}