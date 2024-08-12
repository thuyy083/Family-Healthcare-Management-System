using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.NhanVien.Controllers
{
    public class NhanVienHomeController : Controller
    {
        // GET: NhanVien/NhanVienHome
        csdl_HTQLEntities db = new csdl_HTQLEntities();

        [CustomAuthorize]
        public ActionResult Index()
        {
            var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];
            var NV = db.NHAN_VIEN_CHAM_SOC.Find(taiKhoan.Ma_TK);
            return View(NV);
        }
        [HttpGet]
        [CustomAuthorize]
        public ActionResult SuaThongTin(int? MaTK)
        {
            if (MaTK == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NHAN_VIEN_CHAM_SOC BS = db.NHAN_VIEN_CHAM_SOC.SingleOrDefault(s => s.Ma_TK == MaTK);
            return View(BS);
        }

        // Action để xử lý yêu cầu chỉnh sửa thông tin tài khoản
        [CustomAuthorize]
        [HttpPost, ActionName("SuaThongTin")]
        public ActionResult SuaThongTin(int MaTK)
        {
            var BSUpdate = db.NHAN_VIEN_CHAM_SOC.Find(MaTK);
            if (TryUpdateModel(BSUpdate, "", new string[] { "Ho_Ten", "Ngay_Sinh", "So_CCCD", "Gioi_Tinh", "Dia_Chi", "So_Dien_Thoai", "Kinh_nghiem" }))
            {
                try
                {
                    // Cập nhật thông tin bác sĩ trong bảng BAC_SI_GIA_DINH
                    db.Entry(BSUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan
                    TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(BSUpdate.Ma_TK);
                    if (taiKhoan != null)
                    {
                        // Cập nhật thông tin tài khoản
                        taiKhoan.Ho_Ten = BSUpdate.Ho_Ten;
                        taiKhoan.Ngay_Sinh = BSUpdate.Ngay_Sinh;
                        taiKhoan.So_CCCD = BSUpdate.So_CCCD;
                        taiKhoan.Gioi_Tinh = BSUpdate.Gioi_Tinh;
                        taiKhoan.Dia_Chi = BSUpdate.Dia_Chi;
                        taiKhoan.So_Dien_Thoai = BSUpdate.So_Dien_Thoai;
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
            return View(BSUpdate);
        }
    }
}