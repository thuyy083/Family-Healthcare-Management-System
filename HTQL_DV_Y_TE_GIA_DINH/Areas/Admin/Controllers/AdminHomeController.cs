using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using HTQL_DV_Y_TE_GIA_DINH.Models;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        csdl_HTQLEntities db = new csdl_HTQLEntities();

        // GET: Admin/AdminHome
        [CustomAuthorize]
        public ActionResult Index()
        {
            //var taiKhoan = TempData["TaiKhoanDangNhap"] as TAI_KHOAN;
            var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];
            return View(taiKhoan);
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

            QUAN_TRI_VIEN QTV = db.QUAN_TRI_VIEN.SingleOrDefault(s => s.Ma_TK == MaTK);
            return View(QTV);
        }

        // Action để xử lý yêu cầu chỉnh sửa thông tin tài khoản
        [CustomAuthorize]
        [HttpPost, ActionName("SuaThongTin")]
        public ActionResult SuaThongTin(int MaTK)
        {
            var QTVUpdate = db.QUAN_TRI_VIEN.Find(MaTK);
            if (TryUpdateModel(QTVUpdate, "", new string[] { "Ho_Ten", "Ngay_Sinh", "So_CCCD", "Gioi_Tinh", "Dia_Chi", "So_Dien_Thoai" }))
            {
                try
                {
                    // Cập nhật thông tin quản trị viên trong bảng QuanTriVien
                    db.Entry(QTVUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan
                    TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(QTVUpdate.Ma_TK);
                    if (taiKhoan != null)
                    {
                        // Cập nhật thông tin tài khoản
                        taiKhoan.Ho_Ten = QTVUpdate.Ho_Ten;
                        taiKhoan.Ngay_Sinh = QTVUpdate.Ngay_Sinh;
                        taiKhoan.So_CCCD = QTVUpdate.So_CCCD;
                        taiKhoan.Gioi_Tinh = QTVUpdate.Gioi_Tinh;
                        taiKhoan.Dia_Chi = QTVUpdate.Dia_Chi;
                        taiKhoan.So_Dien_Thoai = QTVUpdate.So_Dien_Thoai;
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
            return View(QTVUpdate);
        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult ChangePassword(int? MaTK)
        {
            if (MaTK == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            QUAN_TRI_VIEN QTV = db.QUAN_TRI_VIEN.SingleOrDefault(s => s.Ma_TK == MaTK);
            return View(QTV);
        }

        // Action để xử lý yêu cầu chỉnh sửa thông tin tài khoản
        [CustomAuthorize]
        [HttpPost, ActionName("ChangePassword")]
        public ActionResult ChangePassword(int MaTK)
        {
            var QTVUpdate = db.QUAN_TRI_VIEN.Find(MaTK);
            if (TryUpdateModel(QTVUpdate, "", new string[] {"Mat_Khau" }))
            {
                try
                {
                    // Cập nhật thông tin quản trị viên trong bảng QuanTriVien
                    db.Entry(QTVUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan
                    TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(QTVUpdate.Ma_TK);
                    if (taiKhoan != null)
                    {
                        // Cập nhật thông tin tài khoản
                        taiKhoan.Ho_Ten = QTVUpdate.Ho_Ten;
                        taiKhoan.Ngay_Sinh = QTVUpdate.Ngay_Sinh;
                        taiKhoan.So_CCCD = QTVUpdate.So_CCCD;
                        taiKhoan.Gioi_Tinh = QTVUpdate.Gioi_Tinh;
                        taiKhoan.Dia_Chi = QTVUpdate.Dia_Chi;
                        taiKhoan.So_Dien_Thoai = QTVUpdate.So_Dien_Thoai;
                        taiKhoan.Mat_Khau = QTVUpdate.Mat_Khau;
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
            return View(QTVUpdate);
        }


        [CustomAuthorize]
        [HttpGet]
        public ActionResult DoiMatKhau()
        {
            Debug.WriteLine("Bat dau");
            return View();
        }

        [CustomAuthorize]
        [HttpPost, ActionName("DoiMatKhau")]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(DoiMatKhauViewModel model)
        {
            Debug.WriteLine("Bat dau");

            if (ModelState.IsValid)
            {
                Debug.WriteLine("ModelState.IsValid");

                var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];

                    if (taiKhoan != null && taiKhoan.Mat_Khau == model.MatKhauCu)
                    {
                        Debug.WriteLine("taiKhoan != null && taiKhoan.Mat_Khau == model.MatKhauCu");
                        taiKhoan.Mat_Khau = model.MatKhauMoi;
                        Debug.WriteLine("MK moi taiKhoan: " + taiKhoan.Mat_Khau);
                        db.SaveChanges();

                        QUAN_TRI_VIEN QTV = db.QUAN_TRI_VIEN.Find(taiKhoan.Ma_TK);
                        if (QTV != null)
                        {
                            // Cập nhật thông tin QTV
                            QTV.Mat_Khau = model.MatKhauMoi;
                            Debug.WriteLine("MK moi QTV: " + QTV.Mat_Khau);

                            Debug.WriteLine("Ma QTV: " + QTV.Ma_TK);

                            // Lưu thay đổi vào cơ sở dữ liệu
                            db.SaveChanges();
                        }

                        // Xóa thông tin đăng nhập của người dùng
                        Session.Remove("TaiKhoanDangNhap");
                        Session.Remove("QuyenTruyCap");

                        TempData["SuccessMessage"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại";
                        // Chuyển hướng người dùng đến trang đăng nhập
                        return RedirectToAction("DangNhap", "Home", new { area = "" });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật khẩu cũ không đúng.");
                    }
                    

            }
            return View(model);
        }

    }
}