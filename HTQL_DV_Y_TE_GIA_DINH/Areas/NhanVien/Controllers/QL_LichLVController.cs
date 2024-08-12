using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using PagedList;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.NhanVien.Controllers
{
    public class QL_LichLVController : Controller
    {
        // GET: NhanVien/QL_LichLV
        csdl_HTQLEntities db = new csdl_HTQLEntities();
        [CustomAuthorize]
        public ActionResult getListLich_LV(int? page)
        {
            var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];
            int maTK_NhanVien = taiKhoan.Ma_TK;

            var listHD = db.HOA_DON
                            .Where(p => p.Ma_TK == maTK_NhanVien)
                            .Select(p => p.So_Hoa_Don)
                            .ToList();

            var listLich = db.LICH_CHAM_SOC
                                .Where(bn => listHD.Contains(bn.Ma_Lich))
                                .OrderByDescending(l => l.Ngay_Bat_Dau);
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(listLich.ToPagedList(pageNumber, pageSize));

        }

        [CustomAuthorize]
        public ActionResult DetailsLCS(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var viewModel = db.LICH_CHAM_SOC.Include(s => s.HOA_DON).SingleOrDefault(s => s.Ma_Lich == id);
            if (viewModel == null)
                if (viewModel == null)
                {
                    return HttpNotFound();
                }
            ViewBag.TaiKhoanList = new SelectList(db.TAI_KHOAN, "Ma_TK", "Ho_Ten");
            return View(viewModel);
        }
    }
}