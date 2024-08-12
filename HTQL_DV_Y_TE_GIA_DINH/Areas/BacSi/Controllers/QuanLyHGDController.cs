using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using PagedList;


namespace HTQL_DV_Y_TE_GIA_DINH.Areas.BacSi.Controllers
{
    public class QuanLyHGDController : Controller
    {
        // GET: BacSi/QuanLyHGD
        csdl_HTQLEntities db = new csdl_HTQLEntities();
        [CustomAuthorize]
        public ActionResult ds_HGD(string searchString, int? page)
        {
            var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];
            int maTK_BacSi = taiKhoan.Ma_TK;

            // Lấy danh sách Hộ gia đình mà bác sĩ được phân công chăm sóc
            var listPC = db.PHAN_CONG_CHAM_SOC
                            .Where(p => p.Ma_TK == maTK_BacSi)
                            .Select(p => p.Ma_HGD)
                            .ToList();

            var listHGD = db.HO_GIA_DINH
                                .Where(bn => listPC.Contains(bn.Ma_HGD));
            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                listHGD = listHGD.Where(bn => bn.Chu_Ho.Contains(searchString));
            }
            listHGD = listHGD.OrderBy(bn => bn.Chu_Ho);
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            ViewBag.CurrentFilter = searchString;
            var pagedHGD = listHGD.ToPagedList(pageNumber, pageSize);
            return View(pagedHGD);
        }
        [CustomAuthorize]
        public ActionResult chitietHGD(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            //Bao gom tat ca danh sach chapter cua boo theo id chi dinh
            var viewModel = db.HO_GIA_DINH.Include(s => s.BENH_NHAN).SingleOrDefault(s => s.Ma_HGD == id);
            if (viewModel == null)
                if (viewModel == null)
                {
                    return HttpNotFound();
                }
            ViewBag.Ma_HGD = id;
            return View(viewModel);
        }

    }
}