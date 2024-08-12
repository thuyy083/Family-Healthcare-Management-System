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

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.BenhNhan.Controllers
{
    public class SuDungDVController : Controller
    {
        // GET: BenhNhan/SuDungDV
        csdl_HTQLEntities db = new csdl_HTQLEntities();
        [CustomAuthorize]
        public ActionResult ds_ThanhVienGD(string searchString, int? page)
        {
            var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];
            //if (taiKhoan != null && taiKhoan.Ma_Quyen == 4)
            //{
                // Lấy mã Hộ gia đình của bệnh nhân đang đăng nhập
                int? maHGD = db.BENH_NHAN.Where(bn => bn.Ma_TK == taiKhoan.Ma_TK).Select(bn => bn.Ma_HGD).FirstOrDefault();

                if (maHGD != null)
                {
                var thanhVienGD = db.BENH_NHAN.Where(bn => bn.Ma_HGD == maHGD);
                if (!String.IsNullOrEmpty(searchString))
                {
                    thanhVienGD = thanhVienGD.Where(s => s.Ho_Ten.Contains(searchString));
                }
                ViewBag.CurrentFilter = searchString;
                thanhVienGD = thanhVienGD.OrderBy(s => s.Ho_Ten);
                int pageSize = 4;
                int pageNumber = (page ?? 1);

                return View(thanhVienGD.ToPagedList(pageNumber, pageSize));
            }
            //}
            return RedirectToAction("Index", "BenhNhanHome");
        }
        [CustomAuthorize]
        public ActionResult getListLCS()
        {
            var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];
            var lichChamSoc = db.LICH_CHAM_SOC.Where(l => l.Ma_TK == taiKhoan.Ma_TK).ToList();
            return View(lichChamSoc);
        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult AddLichChamSoc()
        {
            var model = new LICH_CHAM_SOC();

            // Load danh sách các dịch vụ để hiển thị cho người dùng chọn
            ViewBag.Ma_Dich_Vu = new SelectList(db.DICH_VU_CHAM_SOC_NGUOI_BENH, "Ma_Dich_Vu", "Ten_Dich_Vu");

            return View(model);
        }

        [CustomAuthorize]
        [HttpPost, ActionName("AddLichChamSoc")]
        [ValidateAntiForgeryToken]
        public ActionResult AddLichChamSoc(LICH_CHAM_SOC LCS)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];

                    // Set Trang_Thai mặc định là false
                    LCS.Trang_Thai = false;
                    LCS.Ma_TK=taiKhoan.Ma_TK;
                    db.LICH_CHAM_SOC.Add(LCS);
                    db.SaveChanges();

                    return RedirectToAction("getListLCS"); // Chuyển hướng sau khi thêm thành công
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
            }

            // Load lại danh sách các dịch vụ và nhân viên chăm sóc cho view
            ViewBag.Ma_Dich_Vu = new SelectList(db.DICH_VU_CHAM_SOC_NGUOI_BENH, "Ma_Dich_Vu", "Ten_Dich_Vu", LCS.Ma_Dich_Vu);

            return View(LCS);
        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult XoaLich(int? id)
        {
            LICH_CHAM_SOC LCS = db.LICH_CHAM_SOC.SingleOrDefault(n => n.Ma_Lich == id);

            if (LCS == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            db.LICH_CHAM_SOC.Remove(LCS);

            db.SaveChanges();
            return RedirectToAction("getListLCS");
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

        [CustomAuthorize]
        [HttpGet]
        public ActionResult EditLCS(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LICH_CHAM_SOC LCS = db.LICH_CHAM_SOC.SingleOrDefault(s => s.Ma_Lich == id);

            return View(LCS);
        }

        [CustomAuthorize]
        [HttpPost, ActionName("EditLCS")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditLCS(int id)
        {
            var LCSUpdate = db.LICH_CHAM_SOC.Find(id);
            if (TryUpdateModel(LCSUpdate, "", new string[] { "Ma_Dich_Vu", "Ngay_Bat_Dau", "So_Ngay_Thue", "Yeu_Cau_Chi_Dinh_NVCS", "Ten_NVCS_Muon_Chon", "Mo_Ta_Benh_Ly", "Ho_Ten_Benh_Nhan", "Dia_Chi_Noi_Cham_Soc", "So_Dien_Thoai_Nguoi_Thue" }))
            {
                try
                {
                    // Cập nhật thông tin dịch vụ trong bảng DichVu
                    db.Entry(LCSUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }

                var listDV = db.DICH_VU_CHAM_SOC_NGUOI_BENH.ToList().Select(b => new SelectListItem
                {
                    Value = b.Ma_Dich_Vu.ToString(),
                    Text = b.Ten_Dich_Vu
                }).ToList();
                ViewBag.Ma_Dich_Vu = listDV;

                return RedirectToAction("getListLCS");
            }
            return View(LCSUpdate);
        }

    }
}