using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using PagedList;


namespace HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Controllers
{
    public class QLQTVController : Controller
    {
        // GET: Admin/QLQTV
        csdl_HTQLEntities db = new csdl_HTQLEntities();


        // GET: Admin/QLQTV

        [CustomAuthorize]
        public ActionResult getListQTV(string searchString, int? page)
        {

            var listQTV = from s in db.QUAN_TRI_VIEN select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                listQTV = listQTV.Where(s => s.Ho_Ten.Contains(searchString));
            }
            ViewBag.CurrentFilter = searchString;
            listQTV = listQTV.OrderBy(s => s.Ho_Ten);
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            return View(listQTV.ToPagedList(pageNumber, pageSize));
        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult CreateQTV()
        {
            return View();
        }

        // POST: QuanTriVien/Create
        [CustomAuthorize]
        [HttpPost, ActionName("CreateQTV")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateQTV(QUAN_TRI_VIEN QTV)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tạo một tài khoản mới
                    TAI_KHOAN TK = new TAI_KHOAN
                    {
                        Ma_Quyen = 1,
                        Ho_Ten = QTV.Ho_Ten,
                        Ngay_Sinh = QTV.Ngay_Sinh,
                        So_CCCD = QTV.So_CCCD,
                        Gioi_Tinh = QTV.Gioi_Tinh,
                        Dia_Chi = QTV.Dia_Chi,
                        So_Dien_Thoai = QTV.So_Dien_Thoai,
                        Mat_Khau = QTV.Mat_Khau
                    };
                    db.TAI_KHOAN.Add(TK);
                    db.SaveChanges();

                    int newMaTK = TK.Ma_TK;

                    // Tạo một quản trị viên mới
                    QUAN_TRI_VIEN QuanTriVien = new QUAN_TRI_VIEN
                    {
                        Ma_TK = newMaTK,
                        Ma_Quyen = 1,
                        Ho_Ten = QTV.Ho_Ten,
                        Ngay_Sinh = QTV.Ngay_Sinh,
                        So_CCCD = QTV.So_CCCD,
                        Gioi_Tinh = QTV.Gioi_Tinh,
                        Dia_Chi = QTV.Dia_Chi,
                        So_Dien_Thoai = QTV.So_Dien_Thoai,
                        Mat_Khau = QTV.Mat_Khau,
                    };
                    db.QUAN_TRI_VIEN.Add(QuanTriVien);
                    db.SaveChanges();

                    return RedirectToAction("getListQTV");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Loi luu du lieu");
            }
            var listQTV = from s in db.QUAN_TRI_VIEN select s;
            return View("getlistQTV", listQTV);

        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult EditQTV(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            QUAN_TRI_VIEN QTV = db.QUAN_TRI_VIEN.SingleOrDefault(s => s.Ma_TK == id);
            return View(QTV);
        }

        
        [HttpPost, ActionName("EditQTV")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [CustomAuthorize]
        public ActionResult EditQTV(int id)
        {
            var QTVUpdate = db.QUAN_TRI_VIEN.Find(id);
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

                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }


                return RedirectToAction("getListQTV");
            }
            return View(QTVUpdate);
        }

        //Hiển thị thông tin cần xoá
        [HttpGet]
        [CustomAuthorize]
        public ActionResult DeleteQTV(int? id)
        {
            QUAN_TRI_VIEN QTV = db.QUAN_TRI_VIEN.SingleOrDefault(n => n.Ma_TK == id);

            if (QTV == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // Xóa thông tin quản trị viên trong bảng QUAN_TRI_VIEN
            db.QUAN_TRI_VIEN.Remove(QTV);
            // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan
            TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(QTV.Ma_TK);
            if (taiKhoan != null)
            {
                // Xóa thông tin tài khoản
                db.TAI_KHOAN.Remove(taiKhoan);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();
            return RedirectToAction("getListQTV");
        }
        /*
        public ActionResult DetailsQTV(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            //Bao gom tat ca danh sach chapter cua boo theo id chi dinh
            var viewModel = db.QUAN_TRI_VIEN.Include(s => s.PHAN_CONG_CHAM_SOC).SingleOrDefault(s => s.Ma_TK == id);
            if (viewModel == null)
                if (viewModel == null)
                {
                    return HttpNotFound();
                }
            return View(viewModel);
        }
        */
    }
}