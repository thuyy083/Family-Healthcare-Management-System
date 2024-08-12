using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using HTQL_DV_Y_TE_GIA_DINH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTQL_DV_Y_TE_GIA_DINH.Controllers
{
    public class HomeController : Controller
    {

        csdl_HTQLEntities db = new csdl_HTQLEntities();

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(DangNhapViewModel model)
        {
            if (ModelState.IsValid) // Kiểm tra tính hợp lệ của dữ liệu đầu vào
            {

            
                var taiKhoan = db.TAI_KHOAN.SingleOrDefault(tk => tk.So_CCCD == model.SoCCCD && tk.Mat_Khau == model.MatKhau);
            if (taiKhoan != null)
            {

                //TempData["TaiKhoanDangNhap"] = taiKhoan;
                // Lưu thông tin quyền truy cập vào session
                Session["TaiKhoanDangNhap"] = taiKhoan;

                Session["QuyenTruyCap"] = taiKhoan.Ma_Quyen;

                Session["TenTaiKhoan"] = taiKhoan.Ho_Ten;


                // Chuyển hướng đến khu vực tương ứng
                if (taiKhoan.Ma_Quyen == 1)
                {
                    return RedirectToAction("Index", "AdminHome", new { area = "Admin" }); 
                }
                else if (taiKhoan.Ma_Quyen == 2)
                {
                    return RedirectToAction("Index", "BacSiHome", new { area = "BacSi" });
                }
                else if (taiKhoan.Ma_Quyen == 3)
                {
                    return RedirectToAction("Index", "NhanVienHome", new { area = "NhanVien" });
                }
                else
                {
                    return RedirectToAction("Index", "BenhNhanHome", new { area = "BenhNhan" });
                }
            }
            else
            {
                // Đăng nhập không thành công, chuyển hướng về trang đăng nhập với thông báo lỗi
                TempData["Error"] = "Thông tin đăng nhập không chính xác.";
                return View(model); // Trả về view với thông tin đăng nhập không chính xác

                }
            }
            return View(model);
        }

        public ActionResult DangXuat()
        {
            // Xóa thông tin tài khoản khỏi Session
            Session.Clear(); // Xóa tất cả các giá trị trong session
            Session.Abandon(); // Hủy bỏ session hiện tại

            // Chuyển hướng đến trang đăng nhập hoặc trang chính của ứng dụng
            return RedirectToAction("DangNhap", "Home");
        }

        public ActionResult TrangChu()
        {
            return View();
        }

        public ActionResult GioiThieu()
        {
            return View();
        }

        public ActionResult DichVu()
        {
            var dichVuList = db.DICH_VU_CHAM_SOC_NGUOI_BENH.ToList();

            return View(dichVuList);
        }

        [HttpGet]
        public ActionResult DatLich()
        {
            var model = new LICH_CHAM_SOC();

            // Load danh sách các dịch vụ để hiển thị cho người dùng chọn
            ViewBag.Ma_Dich_Vu = new SelectList(db.DICH_VU_CHAM_SOC_NGUOI_BENH, "Ma_Dich_Vu", "Ten_Dich_Vu");


            return View(model);
        }

        [HttpPost, ActionName("DatLich")]
        [ValidateAntiForgeryToken]
        public ActionResult DatLich(LICH_CHAM_SOC LCS)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    LCS.Trang_Thai = false;
                    db.LICH_CHAM_SOC.Add(LCS);
                    db.SaveChanges();

                    return RedirectToAction("TrangChu");
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

        [HttpGet]
        public ActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost, ActionName("DoiMatKhau")]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(DoiMatKhauViewModel model)
        {
            if (ModelState.IsValid)
            {
                var TK = (TAI_KHOAN)Session["TaiKhoanDangNhap"];

                TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(TK.Ma_TK);

                if (taiKhoan != null && taiKhoan.Mat_Khau == model.MatKhauCu)
                {
                    taiKhoan.Mat_Khau = model.MatKhauMoi;
                    db.SaveChanges();

                    // Xóa thông tin đăng nhập của người dùng
                    Session.Remove("TaiKhoanDangNhap");
                    Session.Remove("QuyenTruyCap");

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



        /*
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(int username, string password)
        {
            if (ModelState.IsValid)
            {
                using (var db = new csdl_HTQLEntities5())
                {
                    var TK = db.TAI_KHOAN.SingleOrDefault(m => m.Ma_TK == username && m.Mat_Khau == password);
                    if (TK != null)
                    {
                        Session["username"] = TK.Ma_TK;
                        ViewBag.username = TK.Ma_TK;
                        TempData["error"] = new TAI_KHOAN();

                        switch (TK.Ma_Quyen)
                        {
                            case 1:
                                return RedirectToAction("Index", "AdminHome", new { area = "Admin" });
                            case 2:
                                return RedirectToAction("Index", "BSHome", new { area = "BacSiGD" });
                            case 3:
                                return RedirectToAction("Index", "NVCSHome", new { area = "NhanVienCS" });
                            default:
                                return RedirectToAction("Index", "BNHome", new { area = "BenhNhan" });
                        }
                    }
                    else
                    {
                        TempData["error"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                        return View();
                    }
                }
            }
            // Nếu ModelState không hợp lệ, trả về lại trang đăng nhập để người dùng nhập lại
            return View();
        }
        */
    }
}