using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.BenhNhan.Controllers
{
    public class BenhNhanHomeController : Controller
    {
        // GET: BenhNhan/BenhNhanHome
        csdl_HTQLEntities db = new csdl_HTQLEntities();

        [CustomAuthorize]
        public ActionResult Index()
        {
            //var taiKhoan = TempData["TaiKhoanDangNhap"] as TAI_KHOAN;
            var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];
            var BN = db.BENH_NHAN.Find(taiKhoan.Ma_TK);
            return View(BN);
        }

        // Action để hiển thị form chỉnh sửa thông tin tài khoản
        [HttpGet]
        [CustomAuthorize]
        public ActionResult SuaThongTin(int? MaTK)
        {
            if (MaTK == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BENH_NHAN BN = db.BENH_NHAN.SingleOrDefault(s => s.Ma_TK == MaTK);
            return View(BN);
        }

        // Action để xử lý yêu cầu chỉnh sửa thông tin tài khoản
        [CustomAuthorize]
        [HttpPost, ActionName("SuaThongTin")]
        public ActionResult SuaThongTin(int MaTK)
        {
            var BNUpdate = db.BENH_NHAN.Find(MaTK);
            if (TryUpdateModel(BNUpdate, "", new string[] { "Ho_Ten", "Ngay_Sinh", "So_CCCD", "Gioi_Tinh", "Dia_Chi", "So_Dien_Thoai", "So_BHYT" }))
            {
                try
                {
                    // Cập nhật thông tin bác sĩ trong bảng BENH_NHAN
                    db.Entry(BNUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan
                    TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(BNUpdate.Ma_TK);
                    if (taiKhoan != null)
                    {
                        // Cập nhật thông tin tài khoản
                        taiKhoan.Ho_Ten = BNUpdate.Ho_Ten;
                        taiKhoan.Ngay_Sinh = BNUpdate.Ngay_Sinh;
                        taiKhoan.So_CCCD = BNUpdate.So_CCCD;
                        taiKhoan.Gioi_Tinh = BNUpdate.Gioi_Tinh;
                        taiKhoan.Dia_Chi = BNUpdate.Dia_Chi;
                        taiKhoan.So_Dien_Thoai = BNUpdate.So_Dien_Thoai;
                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SaveChanges();
                    }
                    // Xóa thông tin đăng nhập của người dùng
                    Session.Remove("TaiKhoanDangNhap");
                    Session.Remove("QuyenTruyCap");

                    // Chuyển hướng người dùng đến trang đăng nhập
                    return RedirectToAction("DangNhap", "Home", new { area = "" });

                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }


                return RedirectToAction("Index");
            }
            return View(BNUpdate);
        }
    }
}